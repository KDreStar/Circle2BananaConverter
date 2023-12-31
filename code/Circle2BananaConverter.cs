using OsuParsers.Beatmaps;
using OsuParsers.Beatmaps.Objects;
using OsuParsers.Beatmaps.Sections.Events;
using OsuParsers.Database.Objects;
using OsuParsers.Decoders;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

public static class Circle2BananaConverter {

    public static Beatmap beatmap = new Beatmap();

    public static Random rand = new Random();

    public static MatchingTable minTable = new MatchingTable();

	public static ConvertOption? option;
    public static void ConvertToBanana(string path, ConvertOption option) {
        /*
        Part 1.
        비트맵 읽기
        Read to beatmap
        */
        beatmap = BeatmapDecoder.Decode(path);
        Circle2BananaConverter.option = option;

		InsertBPM(beatmap);

		beatmap.MetadataSection.Version += "(Converted)";

		beatmap.EventsSection.Breaks = new List<BeatmapBreakEvent>();


		/*
		Part 2.
		DFS (only visit first node)
		*/
		MatchingTable firstNodeDFSedTable = PreProcessing();

        if (option.findMoreRoutes == false) {
			RemoveOverlappedBananas(firstNodeDFSedTable);

			beatmap.HitObjects = new List<HitObject>();

			Save(path, beatmap, firstNodeDFSedTable);
			return; 
        }

        //idk how to optimize this
        //dfs too long
        //bfs memory

        minTable.Set(firstNodeDFSedTable);
        /*
        Test
        */

        SortedHitObjects fruits = new SortedHitObjects(beatmap.HitObjects);
        BananaManager bananaManager = new BananaManager();
        MatchingTable matchingTable = new MatchingTable();

        Console.WriteLine(bananaManager.rngCount);
        Console.WriteLine(firstNodeDFSedTable.GetLastRNGCount());

        while (bananaManager.rngCount < firstNodeDFSedTable.GetLastRNGCount()) {
            (int firstFruitX, int firstFruitTime, int secondFruitX, int secondFruitTime) = Find(bananaManager, fruits);

            //일치하는 경우가 없을 때는 droplet
            if (firstFruitX == -1) {
                bananaManager.ProceedForDroplet();
                continue;
            }

            Debug.WriteLine((bananaManager.rngCount, firstFruitX, firstFruitTime, secondFruitX, secondFruitTime));
            
            matchingTable.Add(new MatchingInfo(bananaManager.rngCount, firstFruitX, firstFruitTime, secondFruitX, secondFruitTime));
            bananaManager.ProceedForDroplet();
        }

		//for (int i = 0; i < 10000; i++)
		//	RandomDFS(fruits, matchingTable, firstNodeDFSedTable);
		
		ReducedMemoryGreedyBFS(matchingTable, fruits.RealCount, 1000);

		RemoveOverlappedBananas(minTable);

		Save(path, beatmap, matchingTable);
        /*
        Part 4.
        Export

        씨앗이 1개 나오는 BPM 기준
        slider_length = slider_multiplier * 50 * dropletCount

        Slider syntax: x,y,time,type,hitSound,curveType|curvePoints,slides,length,edgeSounds,edgeSets,hitSample
        Spinner syntax: x,y,time,type,hitSound,endTime,hitSample
        */

        Console.WriteLine("Complete" + (firstNodeDFSedTable.GetLastRNGCount(), minTable.GetLastRNGCount()));


    }

	private static void Save(string path, Beatmap beatmap, MatchingTable table) {
		string pattern = @"].osu";
		string newFileName = Path.GetFileName(path);

		newFileName = Regex.Replace(newFileName, pattern, " (Converted)].osu");
		string finalPath = Path.Combine(Path.GetDirectoryName(path), newFileName);

		beatmap.Save(finalPath);

		if (File.Exists(finalPath) == false)
			return;

		StreamWriter sw = File.AppendText(finalPath);

		sw.Write(TableToString(table));
		sw.Close();
	}

	private static void InsertBPM(Beatmap beatmap) {
		int i;
		int baseBeatLength = 100;
		int offset;
		int beatLength;

		for (i=0; i<beatmap.TimingPoints.Count; i++) {
			var timingPoint = beatmap.TimingPoints[i];

			offset = timingPoint.Offset;
			beatLength = (int)Math.Round(timingPoint.BeatLength);

			if (offset >= -10000)
				break;
		}

		offset = beatmap.TimingPoints[i].Offset;
		beatLength = (int)Math.Round(beatmap.TimingPoints[i].BeatLength);

		if (offset == -10000 && beatLength == baseBeatLength)
			return;

		var temp = new TimingPoint();

		temp.BeatLength = baseBeatLength;
		temp.Offset = -10000;
		temp.Inherited = false;
		temp.Effects = beatmap.TimingPoints[i].Effects;
		temp.SampleSet = beatmap.TimingPoints[i].SampleSet;
		temp.CustomSampleSet = beatmap.TimingPoints[i].CustomSampleSet;
		temp.Volume = beatmap.TimingPoints[i].Volume;
		temp.TimeSignature = beatmap.TimingPoints[i].TimeSignature;

		beatmap.TimingPoints.Insert(i, temp);
	}

    //첫번째 노드만 돌아서 나온 table 생성의 최솟값을 얻음
    private static MatchingTable PreProcessing() {
        SortedHitObjects fruits = new SortedHitObjects(beatmap.HitObjects);

        BananaManager bananaManager = new BananaManager();

        MatchingTable matchingTable = new MatchingTable();

        while (fruits.RealCount > 0) {
            (int firstFruitX, int firstFruitTime, int secondFruitX, int secondFruitTime) = Find(bananaManager, fruits);

            //일치하는 경우가 없을 때는 droplet
            if (firstFruitX == -1) {
                bananaManager.ProceedForDroplet();
                continue;
            }

            /*
            if (fruits.IsPossible() && firstFruitX == secondFruitX) {
                Console.WriteLine("Same Continued" + fruits.minGap);
                bananaManager.ProceedForDroplet();
                dropletCount++;
                continue;
            }
            */

            //매칭된 경우 기록 (dropletCount, timegap)
            //+ 찾았는데 visit 변화가 없는 경우 = 즉 같은걸 찾은 경우는 스킵
            int prevRealCount = fruits.RealCount;

            fruits.Visit(firstFruitX, firstFruitTime);
            fruits.Visit(secondFruitX, secondFruitTime);

            if (prevRealCount == fruits.RealCount) {
                Console.WriteLine("Continued" + bananaManager.rngCount);
                bananaManager.ProceedForDroplet();
                continue;
            }

            Console.WriteLine("Recording");
            Console.WriteLine((firstFruitX, secondFruitX, bananaManager.rngCount, firstFruitTime, secondFruitTime));
            fruits.Print2();
            Console.WriteLine(fruits.RealCount);

            matchingTable.Add(new MatchingInfo(bananaManager.rngCount, firstFruitX, firstFruitTime, secondFruitX, secondFruitTime));

            bananaManager.ProceedForBanana();
        }

        return matchingTable;
    }

    private static (int, int, int, int) Find(BananaManager bananaManager, SortedHitObjects fruits) {
        int firstBaseX = bananaManager.FirstBananaX;
        int secondBaseX = bananaManager.SecondBananaX;
		int pixelError = option.pixelError;

		int minI = Math.Max(0, firstBaseX - pixelError);
		int maxI = Math.Min(Consts.CATCH_WIDTH, firstBaseX + pixelError);

        int minJ = Math.Max(0, secondBaseX - pixelError);
		int maxJ = Math.Min(Consts.CATCH_WIDTH, secondBaseX + pixelError);

		(int, int, int, int) result = (-1, -1, -1, -1);
        int resultPixelError = pixelError * 2 + 1;

        for (int i=minI; i<=maxI; i++) {
            for (int j=minJ; j<=maxJ; j++) {
                int currentPixelError = Math.Abs(firstBaseX - i) + Math.Abs(secondBaseX - j);

                List<int> firstTimes = fruits.GetPossibleTimes(i);
                List<int> secondTimes = fruits.GetTimes(j);//fruits.IsPossible() ? fruits.GetPossibleTimes(j) : fruits.GetTimes(j);

                (int firstFruitTime, int secondFruitTime) = Match(firstTimes, secondTimes);

                if (firstFruitTime == -1)
                    continue;

				// 결과 픽셀 오차가 가장 작은 경우를 찾음
				if (resultPixelError > currentPixelError) {
					result = (i, firstFruitTime, j, secondFruitTime);
					resultPixelError = currentPixelError;
				}
            }
        }

        return result;
    }


	private static (int, int) Match(List<int> firsts, List<int> seconds) {
		int i = 0;
		int j = 0;

		//Console.WriteLine("Matching " + (firsts.Count + seconds.Count));

		if (firsts.Count == 0 || seconds.Count == 0)
			return (-1, -1);

		/*             i....
        firsts  1001 2000 551313
        seconds 1500 1800 2001 2010
                            j..
        */
		while (i < firsts.Count && j < seconds.Count)
		{
			int firstTime = firsts[i];
			int secondTime = seconds[j];

			//Console.WriteLine((firstTime, secondTime));
			//firstTime > secondTime
			//increase to seconds

			//Console.Write((firstTime, secondTime));
			if (firstTime > secondTime)
				j++;

			//over 100ms gap
			//increase to firsts
			else if (secondTime - firstTime > Consts.MAX_BANANA_GAP)
				i++;

			//100ms
			else
				return (firstTime, secondTime);
		}


		return (-1, -1);
	}


	/*
	 *    3   <--(1개, 2개)
	 *  1
	 *        
	 *        1
	 *     2
	 *     2
	 *  1
	 *       2
	 *    2     1
	 * 1
	 * 
	 * 
	 * 1
	 *   2
	 *     2    <
	 *   2      <-- 둘중에 하나
	 * 1
	 *     
	 * 조건 1
	 * 한 곳에 바나나가 3개 있고 다른 바나나 쌍과 연결되어 있는 경우
	 * x1 | time1 | x2 | time2
	 *  1   100     2    150
	 *  2   150     2    151(150)  <-- will remove
	 *  
	 *  
	 *  ---------------------
	 *  1   100     2    150
	 *  2   150     3    240  <-- will remove
	 *  3   240     4    300
	 *  
	 *  ---------------------
	 *  1   100     2    150
	 *  2   150     3    240  <-- will remove
	 *  4   150     3    240
	 *  
	 *  
	 *  (rngCount, first or second)
	 *  
	 *      O (146, second)
	 *          O (12, second), (146, first)
	 *      O (4, second), (12, first)
	 *  O (4, first)
	 *  
	 *  간단하게 이걸 지울때, 튜플 개수가 1개라도 남으면 지울 수 있음
	 *  바나나 카운트 배열을 만든 다음, 매칭 테이블을 찾으면서 지울 수 있는지 확인;
	 * 
	 */
	private static void RemoveOverlappedBananas(MatchingTable table) {
		//bananCounters[x][time] => bananaCount
		BananaCounter bananaCounter = new BananaCounter();

		//input
		bananaCounter.Counting(table);

		//check
		for (int i=table.infos.Count - 1; i>=0; i--) {
			MatchingInfo info = table.infos[i];

			int x1 = info.firstFruitX;
			int x2 = info.secondFruitX;

			int t1 = info.firstFruitTime;
			int t2 = info.secondFruitTime;

			int prevRealCount = bananaCounter.realCount;

			bananaCounter.Remove(x1, t1);
			bananaCounter.Remove(x2, t2);

			//remove
			if (prevRealCount == bananaCounter.realCount) {
				table.infos.RemoveAt(i);
			} else { //restore
				bananaCounter.Add(x1, t1);
				bananaCounter.Add(x2, t2);
			}
		}
	}

	private static void RandomDFS(SortedHitObjects fruits, MatchingTable baseTable, MatchingTable dfsTable) {
		int min = baseTable.infos[0].rngCount;

		for (int i = 0; i < baseTable.infos.Count; i++) {
			MatchingInfo nextInfo = baseTable.infos[i];

			//처음 것부터 8 차이나는 것까지
			if (nextInfo.rngCount - min >= 8)
				break;

			DFS(fruits, baseTable, dfsTable, i);
		}
	}

	private static void DFS(SortedHitObjects fruits, MatchingTable baseTable, MatchingTable dfsTable, int cur) {
		//step 1. get possible nodes

		List<MatchingInfo> infos = baseTable.infos;
		MatchingInfo currentInfo = infos[cur];
		MatchingInfo nextInfo;

		//더 길면 탐색 종료
		if (currentInfo.rngCount > minTable.GetLastRNGCount())
			return;

		//Console.Write((cur, currentInfo.rngCount));

		dfsTable.Add(currentInfo);

		//exit DFS
		if (fruits.RealCount <= 0) {
			minTable.Set(dfsTable);
			dfsTable.RemoveLast();
			return;
		}

		List<int> possibleNodeIndexs = new List<int>();

		for (int i = cur + 1; i < infos.Count; i++) {
			nextInfo = infos[i];

			//처음 가능한 것 찾기
			if (nextInfo.rngCount - Consts.MAX_RNG_LENGTH >= currentInfo.rngCount) {
				possibleNodeIndexs.Add(i);
				break;
			}
		}

		if (possibleNodeIndexs.Count == 0) {
			dfsTable.RemoveLast();
			return;
		}

		int k = possibleNodeIndexs[0] + 1;

		if (k < infos.Count) {
			MatchingInfo firstPossibleInfo = infos[k];

			for (int i = k; i < infos.Count; i++) {
				nextInfo = infos[i];
				//처음 것부터 8 차이나는 것까지
				if (nextInfo.rngCount - firstPossibleInfo.rngCount >= 8)
					break;

				possibleNodeIndexs.Add(i);
			}
		}

		//Random DFS?
        int randomIndex = rand.Next(0, possibleNodeIndexs.Count);
        int nextIndex = possibleNodeIndexs[randomIndex];

		//Debug.WriteLine((possibleNodeIndexs.Count, randomIndex, nextIndex));

		nextInfo = infos[nextIndex];

        bool firstResult = fruits.Visit(nextInfo.firstFruitX, nextInfo.firstFruitTime);
        bool secondResult = fruits.Visit(nextInfo.secondFruitX, nextInfo.secondFruitTime);

        //DFS
        DFS(fruits, baseTable, dfsTable, nextIndex);

        if (firstResult == true)
            fruits.UnVisit(nextInfo.firstFruitX, nextInfo.firstFruitTime);

        if (secondResult == true)
            fruits.Visit(nextInfo.secondFruitX, nextInfo.secondFruitTime);

		/*
		//Visit
		for (int i = 0; i < possibleNodeIndexs.Count; i++) {
			int possibleIndex = possibleNodeIndexs[i];

			nextInfo = infos[possibleIndex];

			bool firstResult = fruits.Visit(nextInfo.firstFruitX, nextInfo.firstFruitTime);
			bool secondResult = fruits.Visit(nextInfo.secondFruitX, nextInfo.secondFruitTime);

			//DFS
			DFS(fruits, baseTable, dfsTable, possibleIndex);

			if (firstResult == true)
				fruits.UnVisit(nextInfo.firstFruitX, nextInfo.firstFruitTime);

			if (secondResult == true)
				fruits.Visit(nextInfo.secondFruitX, nextInfo.secondFruitTime);
		}
		*/

		dfsTable.RemoveLast();
	}


	private static void BFS(SortedHitObjects fruits, MatchingTable baseTable) {

		//각 정보마다 BananaCounter를 저장
		Queue<List<(int, BananaCounter)>> queue = new Queue<List<(int, BananaCounter)>>();

		while (true) {



		}
	}

	/*
	 * 그리디 하게 선택
	 * 1. (0,2,4) -> (28) -> (58, 59, ...)
	 * 1개로 밖에 노드가 안이어져 있다면 반드시 거기로 가는게 효율적이다.
	 * 
	 * 2. 테이블에 있는 매칭정보로 모든 바나나를 기록 했을 때,
	 * 그 위치의 바나나 개수가 적다면 (1개라면)
	 * 1개면 무조건 선택해야 한다
	 * 2개~ 면 선택을 할 확률이 높아진다.
	 * 
	 * 3. 0, 8, 9, 16 이 있을때
	 * 16에 바나나 1개짜리의 정보
	 * 8에는 바나나 50개짜리의 정보
	 * 9에는 바나나 2개짜리의 정보가 있을때
	 * 
	 * 0->9 로 가버리면 16을 찾지 못하여 답이 안나올 수가 있다
	 * 
	 * => 바나나 빈도수로 테이블을 정렬
	 * 빈도 수가 1인것부터 BFS
	 * 
	 */
	private static void ReducedMemoryGreedyBFS(MatchingTable baseTable, int fruitCount, int maxQueueLength = 100000) {
		BananaCounter bananaFrequency = new BananaCounter();
		BananaCounter bananaCounter = new BananaCounter();

		bananaFrequency.Counting(baseTable);
		bananaFrequency.Print();

		//방문 순서의 index들이 있음
		//ex) [0, 8, 16] [0, 9, 17] [1, 9, 17] ...
		Queue<List<int>> queue = new Queue<List<int>>();

		List<MatchingInfo> infos = baseTable.infos;

		int min = baseTable.infos[0].rngCount;

		for (int i = 0; i < infos.Count; i++) {
			MatchingInfo nextInfo = infos[i];

			//처음 것부터 8 차이나는 것까지
			if (nextInfo.rngCount - min >= 8)
				break;

			queue.Enqueue(new List<int> { i });
		}

		int total = 0;
		int currentMaxRealCount = -1;

		while (queue.Count > 0) {
			int count = queue.Count;
			total++;

			Debug.WriteLine(total);

			PriorityQueue<List<int>, int> resultQueue = new PriorityQueue<List<int>, int>();

			for (int i = 0; i < count; i++) {
				List<int> baseIndexs = queue.Dequeue();
				int currentIndex = baseIndexs[baseIndexs.Count - 1];

				/*
				for (int j=0; j<baseIndexs.Count; j++) {
					Debug.Write(baseIndexs[j] + " ");
				}

				Debug.WriteLine("");
				*/

				MatchingInfo currentInfo = infos[currentIndex];

				//Debug.WriteLine("current: " + currentIndex);

				//바나나 시뮬레이션
				bananaCounter.Counting(baseTable, baseIndexs);

				if (fruitCount == bananaCounter.realCount) {
					Debug.WriteLine("Find");
					minTable.Set(baseTable, baseIndexs);
					return;
					//종료
				}

				int realCount = bananaCounter.realCount;

				//시뮬후에 원래대로
				bananaCounter.UnCounting(baseTable, baseIndexs);

				currentMaxRealCount = Math.Max(currentMaxRealCount, realCount);

				//많지 않은거 그냥 버림
				//if (currentMaxRealCount > realCount)
				//	continue;

				int firstPossibleIndex = infos.Count; //밑에서 못찾으면 그냥 끝

				//rngCount + 8보다 크거나 같은것 중에 가장 작은 index 찾기
				for (int j = currentIndex + 1; j < infos.Count; j++) {
					MatchingInfo nextInfo = infos[j];

					if (nextInfo.rngCount >= currentInfo.rngCount + Consts.MAX_RNG_LENGTH) {
						//Debug.WriteLine("firstPossible:" + j + "nextInfo.rngCount:" + nextInfo.rngCount + "CurrentInfo.count:" + currentInfo.rngCount);
						firstPossibleIndex = j;
						break;
					}
				}

				//찾지 못한 경우
				if (firstPossibleIndex >= infos.Count)
					continue;


				MatchingInfo firstPossibleInfo = infos[firstPossibleIndex];

				//Enqueue
				for (int j = firstPossibleIndex; j < infos.Count; j++) {
					MatchingInfo nextInfo = infos[j];

					int x1 = nextInfo.firstFruitX;
					int t1 = nextInfo.firstFruitTime;
					int x2 = nextInfo.secondFruitX;
					int t2 = nextInfo.secondFruitTime;

					int priority = Math.Min(bananaFrequency.Get(x1, t1), bananaFrequency.Get(x2, t2));

					//처음 rngCount + 8 미만의 값 까지만
					if (nextInfo.rngCount - Consts.MAX_RNG_LENGTH < firstPossibleInfo.rngCount) {
						List<int> newIndexs = new List<int>();

						newIndexs.AddRange(baseIndexs);
						newIndexs.Add(j);

						//작은게 먼저 나와야 작은 것들만 존재함
						//그러므로 -를 붙임
						resultQueue.Enqueue(newIndexs, -priority);
						if (resultQueue.Count > maxQueueLength)
							resultQueue.Dequeue();
					}
				}
			}

			while (resultQueue.Count > 0)
				queue.Enqueue(resultQueue.Dequeue());
		}
	}

	//Too long
	private static void BFSReducedMemoryVer(SortedHitObjects fruits, MatchingTable baseTable) {
		BananaCounter bananaCounter = new BananaCounter();

		//방문 순서의 index들이 있음
		//ex) [0, 8, 16] [0, 9, 17] [1, 9, 17] ...
		Queue<List<int>> queue = new Queue<List<int>>();

		List<MatchingInfo> infos = baseTable.infos;

		int min = baseTable.infos[0].rngCount;

		for (int i = 0; i < infos.Count; i++) {
			MatchingInfo nextInfo = infos[i];

			//처음 것부터 8 차이나는 것까지
			if (nextInfo.rngCount - min >= 8)
				break;

			queue.Enqueue(new List<int> { i });
		}

		int total = 0;

		while (queue.Count > 0) {
			int count = queue.Count;
			total++;

			Debug.WriteLine(total);

			for (int i=0; i<count; i++) {
				List<int> baseIndexs = queue.Dequeue();
				int currentIndex = baseIndexs[baseIndexs.Count - 1];

				/*
				for (int j=0; j<baseIndexs.Count; j++) {
					Debug.Write(baseIndexs[j] + " ");
				}

				Debug.WriteLine("");
				*/

				MatchingInfo currentInfo = infos[currentIndex];

				//Debug.WriteLine("current: " + currentIndex);

				//바나나 시뮬레이션
				bananaCounter.Counting(baseTable, baseIndexs);

				if (fruits.RealCount == bananaCounter.realCount) {
					minTable.Set(baseTable, baseIndexs);
					return;
					//종료
				}

				//시뮬후에 원래대로
				bananaCounter.UnCounting(baseTable, baseIndexs);

				int firstPossibleIndex = infos.Count; //밑에서 못찾으면 그냥 끝

				//rngCount + 8보다 크거나 같은것 중에 가장 작은 index 찾기
				for (int j = currentIndex + 1; j<infos.Count; j++) {
					MatchingInfo nextInfo = infos[j];

					if (nextInfo.rngCount >= currentInfo.rngCount + Consts.MAX_RNG_LENGTH) {
						//Debug.WriteLine("firstPossible:" + j + "nextInfo.rngCount:" + nextInfo.rngCount + "CurrentInfo.count:" + currentInfo.rngCount);
						firstPossibleIndex = j;
						break;
					}
				}

				//찾지 못한 경우
				if (firstPossibleIndex >= infos.Count)
					continue;


				MatchingInfo firstPossibleInfo = infos[firstPossibleIndex];

				//Enqueue
				for (int j = firstPossibleIndex;  j < infos.Count; j++) {
					MatchingInfo nextInfo = infos[j];

					//처음 rngCount + 8 미만의 값 까지만
					if (nextInfo.rngCount - Consts.MAX_RNG_LENGTH < firstPossibleInfo.rngCount) {
						List<int> newIndexs = new List<int>();

						newIndexs.AddRange(baseIndexs);
						newIndexs.Add(j);

						queue.Enqueue(newIndexs);
					}
				}
			}
		}
	}

	/*
	 * 신기술
	 * 슬라이더가 -2147481848 에 위치하면
	 * 끝 지점이 -676ms 인 스피너까지 무시됨 (+ RNG는 늘어남)
	 */
	private static string TableToString(MatchingTable table) {
		BananaTable bananaTable = new BananaTable();
        int currentRNGCount = 0;
        string result = "";

        for (int i=0; i< table.infos.Count; i++) {
            MatchingInfo info = table.infos[i];

            if (info.rngCount > currentRNGCount) {
                int dropletCount = info.rngCount - currentRNGCount;
				

				if (option.reduceOption == ReduceOption.DummySpinner) {
					var bananaInfos = bananaTable.GetAvailableInfos(dropletCount);

					for (int j=0; j< bananaInfos.Count; j++) {
						var bananaInfo = bananaInfos[j];
	
						result += string.Format("256,192,{0},12,0,{1},0:0:0:0:", option.dummySpinnerStartTime - bananaInfo.spinnerLength, option.dummySpinnerStartTime);
						result += Environment.NewLine;

						dropletCount -= bananaInfo.rngCount;
					}
				}

                while (dropletCount > 0) {
                    double baseLength = 100.0 * beatmap.DifficultySection.SliderMultiplier / beatmap.DifficultySection.SliderTickRate;
                    int sliderLength = (int)Math.Round(baseLength * (dropletCount + 1));

                    Console.Write("(" + dropletCount + ")");

                    if (sliderLength > Consts.MAX_SLIDER) {
                        result += string.Format("0,0,{0},2,0,L|1:0,{1},{2}", Consts.MINUS_TIME, 1, Consts.MAX_SLIDER);
                        result += Environment.NewLine;

						dropletCount -= (int)Math.Round((Consts.MAX_SLIDER - baseLength) / baseLength);
                        Console.Write("Exceed " + (int)Math.Round((Consts.MAX_SLIDER - baseLength) / baseLength));
                    } else {
                        result += string.Format("0,0,{0},2,0,L|1:0,{1},{2}", Consts.MINUS_TIME, 1, sliderLength);
                        result += Environment.NewLine;

						dropletCount = 0;
                    }
                }

                currentRNGCount = info.rngCount;
            }

            int firstTime = info.firstFruitTime;
            int secondTime = Math.Max(firstTime + 1, info.secondFruitTime); //if firstTime == secondTime gap+1

            result += string.Format("256,192,{0},12,0,{1},0:0:0:0:", firstTime, secondTime);
            result += Environment.NewLine;

			currentRNGCount += Consts.MAX_RNG_LENGTH;
        }

        return result;
    }
}


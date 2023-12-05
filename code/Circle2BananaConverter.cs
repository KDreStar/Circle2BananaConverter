using OsuParsers.Beatmaps;
using OsuParsers.Decoders;

public static class Circle2BananaConverter {

    public static int pixelError = 4;

    public static Beatmap beatmap = new Beatmap();

    public static Random rand = new Random();

    public static MatchingTable minTable = new MatchingTable();
    public static String ConvertToBanana(string path, int pixelError, bool randomDFS=false) {
        /*
        Part 1.
        비트맵 읽기
        Read to beatmap
        */
        beatmap = BeatmapDecoder.Decode(path);
        Circle2BananaConverter.pixelError = pixelError;

        /*
        Part 2.
        X: indexing
        time: sort (asc)
        */

        MatchingTable firstNodeDFSedTable = PreProcessing();

        string result = "";

        Console.WriteLine(beatmap.ToString());

        if (randomDFS == false) {
            return DFSedTableToString(firstNodeDFSedTable);
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
            (int firstFruitX, int firstFruitTime, int secondFruitX, int secondFruitTime) = Find(bananaManager, fruits, pixelError);

            //일치하는 경우가 없을 때는 droplet
            if (firstFruitX == -1) {
                bananaManager.ProceedForDroplet();
                continue;
            }

            Console.WriteLine((bananaManager.rngCount, firstFruitX, firstFruitTime, secondFruitX, secondFruitTime));
            
            matchingTable.Add(new MatchingInfo(bananaManager.rngCount, firstFruitX, firstFruitTime, secondFruitX, secondFruitTime));
            bananaManager.ProceedForDroplet();
        }

        MatchingTable dfsTable = new MatchingTable();
        for (int i=0; i<100000; i++)
            RandomDFS(fruits, matchingTable, dfsTable);

        result = DFSedTableToString(minTable);
        /*
        Part 4.
        Export

        씨앗이 1개 나오는 BPM 기준
        slider_length = slider_multiplier * 50 * dropletCount

        Slider syntax: x,y,time,type,hitSound,curveType|curvePoints,slides,length,edgeSounds,edgeSets,hitSample
        Spinner syntax: x,y,time,type,hitSound,endTime,hitSample
        */

        Console.WriteLine("Complete" + (firstNodeDFSedTable.GetLastRNGCount(), minTable.GetLastRNGCount()));


        return result;
    }

    //첫번째 노드만 돌아서 나온 table 생성의 최솟값을 얻음
    private static MatchingTable PreProcessing() {
        SortedHitObjects fruits = new SortedHitObjects(beatmap.HitObjects);

        BananaManager bananaManager = new BananaManager();

        MatchingTable matchingTable = new MatchingTable();

        while (fruits.RealCount > 0) {
            (int firstFruitX, int firstFruitTime, int secondFruitX, int secondFruitTime) = Find(bananaManager, fruits, pixelError);

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

    private static (int, int, int, int) Find(BananaManager bananaManager, SortedHitObjects fruits, int pixelError) {
        int firstBaseX = bananaManager.FirstBananaX;
        int secondBaseX = bananaManager.SecondBananaX;

        int minI = firstBaseX - pixelError < 0 ? 0 : firstBaseX - pixelError;
        int maxI = firstBaseX + pixelError > Consts.CATCH_WIDTH ? Consts.CATCH_WIDTH : firstBaseX + pixelError;

        int minJ = secondBaseX - pixelError < 0 ? 0 : secondBaseX - pixelError;
        int maxJ = secondBaseX + pixelError > Consts.CATCH_WIDTH ? Consts.CATCH_WIDTH : secondBaseX + pixelError;

        for (int i=minI; i<=maxI; i++) {
            for (int j=minJ; j<=maxJ; j++) {
                List<int> firstTimes = fruits.GetPossibleTimes(i);
                List<int> secondTimes = false ? fruits.GetPossibleTimes(j) : fruits.GetTimes(j);//fruits.IsPossible() ? fruits.GetPossibleTimes(j) : fruits.GetTimes(j);

                (int firstFruitTime, int secondFruitTime) = Match(firstTimes, secondTimes);

                if (firstFruitTime == -1)
                    continue;

                return (i, firstFruitTime, j, secondFruitTime);
            }
        }

        return (-1, -1, -1, -1);
    }

    private static void RandomDFS(SortedHitObjects fruits, MatchingTable baseTable, MatchingTable dfsTable) {
        int min = baseTable.infos[0].rngCount;

        for (int i=0; i<baseTable.infos.Count; i++) {
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

        for (int i=cur+1; i<infos.Count; i++) {
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
        MatchingInfo firstPossibleInfo = infos[k];

        for (int i=k; i<infos.Count; i++) {
            nextInfo = infos[i];
            //처음 것부터 8 차이나는 것까지
            if (nextInfo.rngCount - firstPossibleInfo.rngCount >= 8)
                break;
            
            possibleNodeIndexs.Add(i);
        }
        
        //Random DFS?
        int randomIndex = rand.Next(0, possibleNodeIndexs.Count);
        int nextIndex = possibleNodeIndexs[randomIndex];
        nextInfo = infos[nextIndex];

        bool firstResult = fruits.Visit(nextInfo.firstFruitX, nextInfo.firstFruitTime);
        bool secondResult = fruits.Visit(nextInfo.secondFruitX, nextInfo.secondFruitTime);

        //DFS
        DFS(fruits, baseTable, dfsTable, nextIndex);

        if (firstResult == true)
            fruits.UnVisit(nextInfo.firstFruitX, nextInfo.firstFruitTime);

        if (secondResult == true)
            fruits.Visit(nextInfo.secondFruitX, nextInfo.secondFruitTime);

        //Visit
        /*
        for (int i=0; i<possibleNodeIndexs.Count; i++) {
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

    private static void RandomBFS() {

    }

    private static string DFSedTableToString(MatchingTable dfsedTable) {
        int currentRNGCount = 0;
        string result = "";

        for (int i=0; i<dfsedTable.infos.Count; i++) {
            MatchingInfo info = dfsedTable.infos[i];

            /*
            idk
            how it works at 3 billions ms

            hard coding

            BPM 100
            length | repeat | dropletCount
                50 | 1      | 1 tick 0
                50 | 2      | 2
                50 | 3      | 4
                50 | 4      | 5
                50 | 5      | 6
                50 | 6      | 8
                50 | 7      | 9
                50 | 8      | 10
                50 | 9      | 12
                50 | 10     | 13
                50 | 11     | 15
                50 | 12     | 16

                ..

                50 | 44 | 


                60 | 1      | 2
                60 | 2      | 3
                60 | 9      | 16


                100 | 1     | 3 tick 0
                100 | 2     | 7
                100 | 3     | 11
                100 | 4     | 14

                50 | 1      | 1 tick 0

                100 | 1     | 3 tick 0

                150 | 1     | 6 tick 1   / 3 / 2 

                200 | 1     | 8 tick 1  / 3 / 4?

                250 | 1     | 10 tick 2   3 3 2

                300 | 1     | 13 tick 2   3 3 4

                350 | 1     | 15 tick 3   3 3 3 3

                400 | 1     | 17 tick 3   3 3 4 4

                450 | 1     | 20 tick 4   3 3 3 3 4

                500 | 1     | 22 tick 4   3 3 4 4 4

                550 | 1     | 25 tick 5

                if (sinceLastTick > 80) {
                    double timeBetweenTiny = sinceLastTick;

                    while (timeBetweenTiny > 100)
                        timeBetweenTiny /= 2;

                    for (double t = timeBetweenTiny; t < sinceLastTick; t += timeBetweenTiny)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        AddNested(new TinyDroplet
                        {
                            StartTime = t + lastEvent.Value.Time,
                            X = ClampToPlayfield(EffectiveX + Path.PositionAt(
                                lastEvent.Value.PathProgress + (t / sinceLastTick) * (e.PathProgress - lastEvent.Value.PathProgress)).X),
                        });
                    }
                }

                idk

                59 1
                60 2
                ~
                71 2
                72 inf loop
                ~
                91 inf loop
                92 3
                ~
                101 3
                102 5
                ~
                139 5
                140 inf loop



                L  | R  | D
                50 | 1  | 1
                50 | 2  | 2
                50 | 3  | 4
                50 | 6  | 8
                50 | 12 | 16
                50 | 24 | 32
                50 | 48 | 64


                BPM 200 tickRate = 2

                1 0
                ~
                33 0
                33.333 0
                33.34 inf <- 100 / 3
                34 inf
                ~
                53 inf
                53.3 inf
                53.4 4
                54 4
                ~
                56 4 
                57 6
                66 6

                BPM 200 tickRate = 1

                1. 틱이 생성이 안되는 거리면 무한루프

                -214700000
                -2147000000
                -2147481300
                -2147481400

                -2147481835 .. .
                -2147481836 . ..

                -2147481844 . .
                -2147481847 (max)
                -2147481848 (skiped)

                작은 씨앗때문에 생기는 오류 같으므로
                BPM=600 으로 하고 큰 씨앗만 생성되게 하기

                384 = a * b

                tickRate | min sliderlength for it is showed 1 big droplet
                       1 | 100
                       2 | 50
                       3 | 33.3
                       4 | 25

                if (repeat count * slider length > 60000)
                    fail to load the beatmap
            */

            /* 
            시작 0

            테이블이 4 라면

            droplet 4개 추가
            */
            if (info.rngCount > currentRNGCount) {
                int dropletCount = info.rngCount - currentRNGCount;

                while (dropletCount > 0) {
                    double baseLength = 100.0 * beatmap.DifficultySection.SliderMultiplier / beatmap.DifficultySection.SliderTickRate;
                    int sliderLength = (int)Math.Round(baseLength * (dropletCount + 1));

                    Console.Write("(" + dropletCount + ")");

                    if (sliderLength > Consts.MAX_SLIDER) {
                        result += string.Format("0,0,{0},2,0,L|1:0,{1},{2}", Consts.MINUS_TIME, 1, Consts.MAX_SLIDER);
                        result += "\n";

                        dropletCount -= (int)Math.Round((Consts.MAX_SLIDER - baseLength) / baseLength);
                        Console.Write("Exceed " + (int)Math.Round((Consts.MAX_SLIDER - baseLength) / baseLength));
                    } else {
                        result += string.Format("0,0,{0},2,0,L|1:0,{1},{2}", Consts.MINUS_TIME, 1, sliderLength);
                        result += "\n";

                        dropletCount = 0;
                    }
                }

                currentRNGCount = info.rngCount;
            }

            int firstTime = info.firstFruitTime;
            int secondTime = Math.Max(firstTime + 1, info.secondFruitTime); //if firstTime == secondTime gap+1

            result += string.Format("256,192,{0},12,0,{1},0:0:0:0:", firstTime, secondTime);
            result += "\n";

            currentRNGCount += Consts.MAX_RNG_LENGTH;
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
        while (i < firsts.Count && j < seconds.Count) {
            int firstTime = firsts[i];
            int secondTime = seconds[j];

            //Console.WriteLine((firstTime, secondTime));
            //firstTime > secondTime
            //increase to seconds

            //Console.Write((firstTime, secondTime));
            if (firstTime > secondTime) {
                j++;
            }

            //over 100ms gap
            //increase to firsts
            else if (secondTime - firstTime > Consts.MAX_BANANA_GAP) {
                i++;
                
            }

            //100ms
            else {
                return (firstTime, secondTime);
            }
        }


        return (-1, -1);
    }
}


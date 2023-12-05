# Circle2BananaConverter

## 설명
서클들로 바나나의 지점을 표시한 비트맵이 있으면   
서클을 바나나로 전환 시키는 프로그램입니다.   


## 요구사항
require to [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) later version
[.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) 이상의 버전을 필요로 합니다.

## 사용법
### 비트맵 설정
- 결과를 붙여넣을 비트맵과 전환시킬 비트맵의 SliderTickRate, SliderMultiplier 값이 각각 같아야 합니다.   

- 결과를 붙여넣을 비트맵에서 -10000ms 에 600 BPM 타이밍 포인트를 생성해야 합니다.

### 프로그램 설명
- Beatmap path   
서클을 바나나로 전환할 비트맵의 경로입니다.   

- Browse   
버튼을 누르면 파일을 선택할 수 있는 창을 띄웁니다.   

- Allowed pixel error   
바나나가 배치될 때, 기존 서클의 x위치에서 얼마만큼의 오차를 허용할 것인지에 대한 값입니다.   
작은 값을 가지면 탐색 시간이 길어집니다.   
   
예시   
pixelError = 5 인 경우   
x - 5 <= x <= x + 5 까지 허용

- Convert   
버튼을 누르면 비트맵 파일을 전환시킵니다.

- Find another random routes   
체크시, 비트맵을 전환 시킨 후에 바나나의 순서를 다르게 하여 배치 시킬 수 있는지 탐색합니다.   

- Open result.txt   
체크시, 전환이 끝나면 결과 텍스트를 엽니다.

- Reduce overlapped bananas   
체크시, 바나나 겹침을 줄이게 탐색합니다.

### Ctrl+V
result.txt
```
0,0,-2147481848,2,0,L|1:0,1,220
256,192,96026,12,0,96107,0:0:0:0:
0,0,-2147481848,2,0,L|1:0,1,220
256,192,122599,12,0,122639,0:0:0:0:
256,192,92196,12,0,92276,0:0:0:0:
256,192,24495,12,0,24576,0:0:0:0:

...
```

```

...
[TimingPoints]
...

[HitObjects]
<- Ctrl + V

```

## 원리
### 바나나 개수
[https://github.com/ppy/osu/blob/master/osu.Game.Rulesets.Catch/Objects/BananaShower.cs](https://github.com/ppy/osu/blob/master/osu.Game.Rulesets.Catch/Objects/BananaShower.cs)

```c#
private void createBananas(CancellationToken cancellationToken)
{
    double spacing = Duration;
    while (spacing > 100)
        spacing /= 2;

    if (spacing <= 0)
        return;

    double time = StartTime;
    int i = 0;

    while (time <= EndTime)
    {
        cancellationToken.ThrowIfCancellationRequested();

        AddNested(new Banana
        {
            StartTime = time,
            BananaIndex = i,
            Samples = new List<HitSampleInfo> { new Banana.BananaHitSampleInfo(CreateHitSampleInfo().Volume) }
        });

        time += spacing;
        i++;
    }
}
```

|길이|바나나 개수|
|---:|:---------|
| 1 ~ 100ms | 2 |
| 101ms ~ 200ms | 3 |
| 201ms ~ 400ms | 5 |
| 401ms ~ 800ms | 9 |
| 801ms ~ 1600ms | 17 |
| ...            | ... |
| T(n-1) + 1ms ~ Tms | A(n-1) * 2 - 1 |

T(0) = 0ms   
T(1) = 100ms   
A(0) = 2   

T(n) = T(n-1) * 2 = 100 * 2^n   
A(n) = A(n-1) * 2 - 1 = 2^n + 1   
   
바나나 개수가 많아질 수록 서클과 일치하는 바나나를 찾기가 힘들어지므로   
바나나가 최소로 나오는 1 ~ 100ms 의 시간 차이가나는 서클들만 탐색하여 시간을 줄일 수 있습니다.   


### 바나나 생성
[https://github.com/ppy/osu/blob/master/osu.Game.Rulesets.Catch/Beatmaps/CatchBeatmapProcessor.cs](https://github.com/ppy/osu/blob/master/osu.Game.Rulesets.Catch/Beatmaps/CatchBeatmapProcessor.cs)   

```c#
case BananaShower bananaShower:
    foreach (var banana in bananaShower.NestedHitObjects.OfType<Banana>())
    {
        banana.XOffset = (float)(rng.NextDouble() * CatchPlayfield.WIDTH);
        rng.Next(); // osu!stable retrieved a random banana type
        rng.Next(); // osu!stable retrieved a random banana rotation
        rng.Next(); // osu!stable retrieved a random banana colour
    }

    break;

case JuiceStream juiceStream:
    // Todo: BUG!! Stable used the last control point as the final position of the path, but it should use the computed path instead.
    lastPosition = juiceStream.OriginalX + juiceStream.Path.ControlPoints[^1].Position.X;

    // Todo: BUG!! Stable attempted to use the end time of the stream, but referenced it too early in execution and used the start time instead.
    lastStartTime = juiceStream.StartTime;

    foreach (var nested in juiceStream.NestedHitObjects)
    {
        var catchObject = (CatchHitObject)nested;
        catchObject.XOffset = 0;

        if (catchObject is TinyDroplet)
            catchObject.XOffset = Math.Clamp(rng.Next(-20, 20), -catchObject.OriginalX, CatchPlayfield.WIDTH - catchObject.OriginalX);
        else if (catchObject is Droplet)
            rng.Next(); // osu!stable retrieved a random droplet rotation
    }

    break;
```

비트맵 처리시 바나나를 만나면 RNG를 4번 사용하고   
씨앗을 만나면 RNG를 1번만 사용합니다.   
   
RNG에 사용하는 시드값은 1337 입니다.   

### HitObjects 처리
```
[HitObjects]
256,192,800,12,0,1600,0:0:0:0: //800ms ~ 1600ms Spinner
256,192,0,12,0,800,0:0:0:0:    //0ms   ~  800ms Spinner
```

```
[HitObjects]
256,192,0,12,0,800,0:0:0:0:    //0ms   ~  800ms Spinner
256,192,800,12,0,1600,0:0:0:0: //800ms ~ 1600ms Spinner
```

사진을 보면 결과가 다른 것을 볼 수 있습니다.   
이는, 파일 처리시 정렬하지 않고 줄 그대로 처리 하는 것을 알 수 있습니다.   
   
이것으로 노트에 맞는 바나나를 순서대로 찾기위해서 탐색을 하지않고
바나나의 위치를 먼저 알아낸 뒤에 모든 노트들을 탐색하여 일치하는 노트들을 찾아내는 방식으로 할 수 있습니다.
   
(예시)



### 음수 시간
노트를 특정 음수 시간대에 배치를 하면 무한루프에 걸리거나 노트가 나오지 않고 무시됩니다.   
   
스피너는 대략 -17000000ms 에 배치를 하면 무한루프에 걸리고   
대략 -2147483648ms 에 배치를 하면 스피너가 무시됩니다.   
이때, 무시된 스피너에서는 RNG를 사용하지 않습니다.   
   
슬라이더는 -2147481847ms ~ 0ms 에 배치를 하면 노트가 그대로 나옵니다.   
(-2147481847ms 보다 큰값에서 근처에 배치를 하면 과일이 흐물흐물하게 나오고 씨앗 생성 매커니즘이 달라집니다.)   
-2147481848ms 부터 배치를 하면 노트가 무시되지만 RNG는 사용됩니다.   
   
즉, 이 시간대에 슬라이더를 적절히 배치후 스피너를 원하는 곳에 배치를 하면 서클을 바나나로 전환 시킬 수 있습니다.


### 음수 시간대 슬라이더
TO DO
   

### 탐색 알고리즘
TO DO
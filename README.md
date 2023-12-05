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
TODO

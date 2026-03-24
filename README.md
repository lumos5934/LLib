# ✨lumos-library-core
유니티 기반 & 에셋에서 자주 사용되는 요소, <br>
또는 확장하는 기능의 스크립트를 모아놓아 개발의 생산성 상승을 위한 유니티 패키지

<br>

## ℹ️의존성

* **URP**
* **Newtonsoft.Json (자동 설치)**
* [ UniTask ](https://github.com/Cysharp/UniTask?tab=readme-ov-file#upm-package)
* [ Tri-Inspector ](https://github.com/codewriter-packages/Tri-Inspector?tab=readme-ov-file)

<br>

## ℹ️확장
* [ lumos-DOTween ](https://github.com/lumos5934/lumos-DOTween)
* [ lumos-firebase ](https://github.com/lumos5934/lumos-firebase)
* [ lumos-BGDatabase ](https://github.com/lumos5934/lumos-BGDatabase)

<br>

## ℹ️기능

* [ Audio ](#Audio)
* [ Event ](#Event)
* [ FSM ](https://www.notion.so/FSM-2df3966a742c8021b453c80bab800d3f?source=copy_link)
* [ Global ](https://www.notion.so/Global-2df3966a742c802d9ca9ffb15b215c58?source=copy_link)
* [ Input ](https://www.notion.so/Input-2df3966a742c80829b9fd7302ad19cdb?source=copy_link)
* [ PreInitialize ](https://www.notion.so/PreInitialize-2df3966a742c800185e4d1920150c6a3?source=copy_link)
* [ Pool ](https://www.notion.so/Pool-2df3966a742c8066b614c44f459abde8?source=copy_link)
* [ Resource ](https://www.notion.so/Resource-2df3966a742c80bbbad3d8fbf0bd24d2?source=copy_link)
* [ Save ](https://www.notion.so/Save-2df3966a742c80898b8ad7cd3d16f9ec?source=copy_link)
* [ Tutorial ](https://www.notion.so/Tutorial-2df3966a742c808b860be13cf2e99a08?source=copy_link)
* [ TestWindow ](https://www.notion.so/Test-Editor-2df3966a742c80c3af6ac96904d157da?source=copy_link)
* [ Popup ](https://www.notion.so/UI-2df3966a742c80f38990cbc42a6d1b49?source=copy_link)

<br>
<br>

---

### Audio

**AudioManager** <br>
`Create / [LumosLib] / Prefabs / Manager / Audio`

오디오 통합 관리자 <br>

<table>
  <tr>
    <td><b>Mixer</b></td>
    <td>사용할 오디오 믹서</td>
  </tr>
  <tr>
    <td><b>AudioPlayer</b></td>
    <td>재생에 사용되는 프리팹</td>
  </tr>
   <tr>
    <td><b>SetVolume()</b></td>
    <td>사용되는 믹서 볼륨 조절</td>
  </tr>
   <tr>
    <td><b>BGM</b></td>
    <td>`bgmType`별 독립 채널 관리 (전투, 환경음 등)</td>
  </tr>
   <tr>
    <td><b>SFX</b></td>
    <td>추적이 필요 없는 단발성 효과음</td>
  </tr>
</table>



<br>

**AudioPlayer** <br>
`Create / [LumosLib] / Prefabs / Audio Player`

매니저를 통해 관리되는 오디오 플레이어


<br>

**Sound Asset** <br>
`Create / [LumosLib] / Scriptable Objects / SoundAsset`

사용되는 사운드 보관 SO

<table>
  <tr>
    <td><b>MixerGroup</b></td>
    <td>사용될 믹서그룹</td>
  </tr>
  <tr>
    <td><b>Clip</b></td>
    <td>사용될 오디오 클립</td>
  </tr>
   <tr>
    <td><b>VolumeMult</b></td>
    <td>볼륨 가중치</td>
  </tr>
   <tr>
    <td><b>IsLoop</b></td>
    <td>반복 여부</td>
</table>

<br>

[🎬튜토리얼](https://youtu.be/h66xEmaztBA?si=_H5PhyZfN-9ZT5Gh)


<br>
<br>

---

### EventBus
객체 간의 결합도를 낮추기 위한 전역 이벤트 시스템, 반드시 적절히 Unsubscribe를 호출하여 메모리 누수 방지

<table>
  <tr>
    <td><b>Subscribe()<b></td>
    <td>이벤트 등록</td>
  </tr>
  <tr>
    <td><b>Unsubscribe()<b></td>
    <td>이벤트 해제</td>
  </tr>
  <tr>
    <td><b>Publish()<b></td>
    <td>이벤트 발행</td>
  </tr>
</table>

```csharp
EventBus<LevelUpEvent>.Subscribe(OnLevelUp);
EventBus<LevelUpEvent>.Publish(new LevelUpEvent { Level = 10 });
```

<br>
<br>

---

### FSM

**StateMachine**

<table>
  <tr>
    <td><b>CurState<b></td>
    <td>현재 상태</td>
  </tr>
  <tr>
    <td><b>OnExit<b></td>
    <td>상태 Exit 콜백</td>
  </tr>
  <tr>
    <td><b>OnEnter<b></td>
    <td>상태 Enter 콜백</td>
  </tr>
  <tr>
    <td><b>Register(IState state)<b></td>
    <td>상태 등록</td>
  </tr>
  <tr>
    <td><b>Update()<b></td>
    <td>현재 상태 업데이트</td>
  </tr>
  <tr>
    <td><b>ChangeState()<b></td>
    <td>상태 변경</td>
  </tr>
</table>

```csharp
public class PlayerController : MonoBehaviour
{
    private StateMachine _stateMachine = new StateMachine();

    private void Start()
    {
        // 1. 상태 등록 (Register)
        _stateMachine.Register(new IdleState());
        _stateMachine.Register(new MoveState());

        // 2. 초기 상태 설정
        _stateMachine.ChangeState<IdleState>();
    }

    private void Update()
    {
        // 3. 현재 상태 업데이트 실행
        _stateMachine.Update();

        // 예시: 입력에 따른 상태 변경
        if (Input.GetKeyDown(KeyCode.W))
        {
            _stateMachine.ChangeState<MoveState>();
        }
    }
}
```

<br>

**IState**

상태머신이 관리할 상태, 상속을 통한 상태 구현

```csharp
public class IdleState : IState
{
    public void OnEnter() => Debug.Log("대기 상태 진입");
    public void Update() { /* 대기 로직 */ }
    public void OnExit() => Debug.Log("대기 상태 종료");
}

public class MoveState : IState
{
    public void OnEnter() => Debug.Log("이동 상태 진입");
    public void Update() { /* 이동 로직 */ }
    public void OnExit() => Debug.Log("이동 상태 종료");
}

```


<br>
<br>

---

## ℹ️사전 작업

![Preload](https://github.com/user-attachments/assets/5bb381a1-24b1-407c-8f56-ebd1e4dc6224)
![GetAsyncManager](https://github.com/user-attachments/assets/95862b4c-4cd2-432b-b358-bad5d98c0cf4)

> [!NOTE]
> 컴파일시 Resources 에 자동 생성 혹은 직접 생성한 LumosLibSetting 을 통해 <br>
> 사전 생성할 오브젝트를 추가, 초기화 할 수 있습니다. <br>
> Use 체크를 통해 사전 생성, 초기화를 실행 할 지 선택 할 수 있으며, <br>
> 어느 씬에서든지 런타임시 비동기적으로 사전 초기화를 진행합니다.

<br>


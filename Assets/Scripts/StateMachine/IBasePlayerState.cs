using System.Collections;
using System.Collections.Generic;
public interface IBasePlayerState  {

    void Update();
    void HandleInput();
    void OnStateEnter();
    void OnStateExit();



}

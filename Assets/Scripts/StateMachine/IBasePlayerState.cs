using System.Collections;
using System.Collections.Generic;
public interface IBasePlayerState  {

    void OnStateEnter();
    void HandleInput();
    void Update();
    void FixedUpdate();
    void OnStateExit();



}

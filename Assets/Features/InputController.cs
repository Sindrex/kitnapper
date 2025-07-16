using UnityEngine;

public enum InputPurpose
{
    MOVE_UP, MOVE_DOWN, MOVE_LEFT, MOVE_RIGHT,
    INTERACT,
    ESCAPE_MENU,
    DIALOGUE_CHOICE_UP,
    DIALOGUE_CHOICE_DOWN,
    ANY,
    QUIT, RESET
}

public static class InputController
{
    public static bool InputEnabled;

    public static bool GetInput(InputPurpose purpose)
    {
        if(!InputEnabled)
        {
            return false;
        }

        switch (purpose)
        {
            case InputPurpose.MOVE_UP:
                return Input.GetKey(KeyCode.W);
            case InputPurpose.MOVE_DOWN:
                return Input.GetKey(KeyCode.S);
            case InputPurpose.MOVE_LEFT:
                return Input.GetKey(KeyCode.A);
            case InputPurpose.MOVE_RIGHT:
                return Input.GetKey(KeyCode.D);
            case InputPurpose.INTERACT:
                return Input.GetKeyDown(KeyCode.E);
            case InputPurpose.ESCAPE_MENU:
                return Input.GetKeyDown(KeyCode.Escape);
            case InputPurpose.DIALOGUE_CHOICE_UP:
                return Input.GetKeyDown(KeyCode.W);
            case InputPurpose.DIALOGUE_CHOICE_DOWN:
                return Input.GetKeyDown(KeyCode.S);
            case InputPurpose.ANY:
                return Input.anyKeyDown;
            case InputPurpose.QUIT:
                return Input.GetKeyDown(KeyCode.Q);
            case InputPurpose.RESET:
                return Input.GetKeyDown(KeyCode.R);
        }
        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StatusCodes 
{
    public const int A_OK = 0;
    
    // i guess ill start with Input System Realted status
    public const int CONTROL_SCHEME_NOT_FOUND = -1;
    public const int CONTROL_SCHEME_IN_USE = -2;
    public const int BINDING_IS_EMPTY = -3;
    public const int FAILED_TO_GET_VALID_INPUT_CONTROL_SCHEME = -4;
    public const int BINDING_IS_NULL = -5;
    public const int BINDING_HAS_NO_VALUE = -6;

    #region Camera_System_Related

    public const int NO_CAMERA_IN_SCENE = -100;

    #endregion

    #region General_Errors_And_Warnings

    public const int OPTION_NOT_SETUP_YET = -200;
    public const int VALUE_NOT_FOUND_IN_ARRAY = -201;


    #endregion
}

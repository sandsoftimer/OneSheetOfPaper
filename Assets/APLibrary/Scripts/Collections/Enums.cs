public enum LoadSceneType
{
    LOAD_BY_NAME,
    LOAD_BY_INDEX
}

public enum GameState
{
    NONE, // This is a whistler to all APBehaviour that APManager just created.
    GAME_DATA_LOADED, // Game wii start loading gameplay existing/new data in this state, this state will call once in a scene
    GAME_INITIALIZED, // After loading gameplay SAVE DATA this state will be called, Game will be ready to play with previous data
    GAME_PLAY_STARTED,
    GAME_PLAY_ENDED,
    GAME_PLAY_PAUSED,
    GAME_PLAY_UNPAUSED
}

public enum AcceptedValueType
{
    BOTH,
    ONLY_POSITIVE,
    ONLY_NEGETIVE,
    NONE
}

public enum TappingType
{
    NONE,
    TAP_START,
    TAP_END,
    TAP_N_HOLD
}

public enum SwippingType
{
    SWIPE_UP,
    SWIPE_DOWN,
    SWIPE_LEFT,
    SWIPE_RIGHT
}

public enum UpdateMethod
{
    UPDATE,
    FIXED_UPDATE
}

public enum APAxis
{
    ALL,
    X,
    Y,
    Z
}

public enum MeshUsingType
{
    ORDINARY,
    CUTTING,
    DEFORMING
}

public enum Pivot
{
    TOP_LEFT,
    TOP_CENTER,
    TOP_RIGHT,
    MIDDLE_LEFT,
    MIDDLE_CENTER,
    MIDDLE_RIGHT,
    BOTTOM_LEFT,
    BOTTOM_CENTER,
    BOTTOM_RIGHT
}
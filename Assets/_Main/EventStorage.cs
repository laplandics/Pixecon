public abstract class Event {}

public class LoadMenuEvent : Event {}
public class LoadGameEvent : Event {}

public class GameSceneLoadedEvent : Event {}

public class GameBoxDestroyedEvent : Event { public GameBox DestroyedGb; }

public class PauseGameEvent : Event {}
public class ResumeGameEvent : Event {}

public class CorrectLetterEnteredEvent : Event { public char Letter; }
public class WordLettersDoneEvent : Event {}
public class GameWonEvent : Event {}
public class GameOverEvent : Event {}
public class GameExitEvent : Event {}
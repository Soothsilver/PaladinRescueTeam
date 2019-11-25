/*! \mainpage Welcome to Auxiliary 3 documentation!
\section intro Summary
"Auxiliary" is an XNA library of classes and functions. It consists of two parts: The main "Auxiliary" class library that works only with XNA 4, and the "Cother" class library which consists of general C# classes not requiring XNA. The primary aim of the Auxiliary library is to help you develop simple XNA Windows games faster, eliminating much of the legwork you must normally do. For example, the Auxiliary library has a static class called "Primitives" with methods such as "DrawRectangle", "DrawAndFillRoundedRectangle" or "DrawMultiLineText" which allow you to draw graphics primitives. Besides some auxiliary functions it has, the other important part of the library is its system of GamePhases. These will allow you to layer parts of your UI on top of each other, drawing all of them, but allowing the user to interact only with the topmost. Typically, you would have a "MainIngamePhase" which would do most of the work, but on top of that, you could have a "SelectABuildingPhase" which is a dialog window that prompts the user to choose a building to create. You would want the user to see both the main game map in the background and the new window in the front. Auxiliary supports this.

\section install Creating a project that uses Auxiliary
To fully utilize the Auxiliary library, notably its GamePhase and Primitives classes, you must first take these steps:

1. Add the "Auxiliary", "AuxiliaryContent" and "Cother" projects to your solution.
2. Add "Auxiliary" and "Cother" as references to your project.
3. At program startup, call the Root.Init() static method with appropriate parameters (see the example game, HelloAuxiliary).
4. In your main Draw() method, add these lines:

    spriteBatch.Begin();
    Root.DrawPhase(gameTime);
    Root.DrawOverlay(gameTime);
    spriteBatch.End();

where "spriteBatch" is the sprite batch passed to the Root.Init() method.

5. In your main Update() method, add this line:

    Root.Update(gameTime);

6. You should now be able to use all classes and methods of the library.

\section moreinfo How the GamePhase system works
If you want to use the library's GamePhase system, this is what you need to know:
Each screen in your game (for example, MainMenu, LevelSelection, InGame, LoadGameDialog or ConfirmationDialog) may be represented as a derived class of GamePhase. You may then use Root.PushPhase to add an instance of the screen to the PhaseStack and Root.PopFromPhase() to remove.
In Root.DrawPhase(), all phases in the stack will be drawn using their Draw() method.
In Root.Update() only the topmost game phase on the stack will be update using its Update() method.

\section example Example
This code:
 
    Primitives.DrawAndFillRoundedRectangle(
        new Rectangle(10, 10, 400, 50),
        Color.Cyan,
        Color.Black,
        5);
    Primitives.DrawMultiLineText(
        "Hello, Auxiliary!",
        new Rectangle(10, 10, 400, 50),
        Color.Black,
        Library.FontVerdana,
        Primitives.TextAlignment.Middle);

draw a rounded rectangle, filled with Cyan, and with a 5-px black border and puts the text "Hello, Auxiliary!" centered inside the rectangle. The text will be word-wrapped if exceeding the rectangle width and will be drawn using the default spritefont of Auxiliary, FontVerdana.
See the "HelloAuxiliary" example project for more.





















*/
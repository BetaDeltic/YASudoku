# YASudoku

## Structure
There are only two projects - the YASudoku.csproj in the root and YASudoku.Tests.csproj under YASudoku.Tests\

### Important parts
Most of the logic for new game generation is under Models\PuzzleGenerator, Models\PuzzleResolver and Models\PuzzleValidator.

Most of the UI logic is under ViewModels\GameViewModel\.

### Less important parts
Common\ contains universal code like class Extensions

Controls\ contains game specific Controls like a game cell, that can display either single number or a grid

The rest should be fairly straight-forward

## Building
You may have to run "dotnet restore" before the first build. Otherwise there should be no issues building and running the windows configuration. (I'm unable to test on independent machine at this point)

# Chess Bot CS

## BOOKMARKS  
- [OBJECTIVE](#objective)  
- [REASON](#reason)  
- [LICENSE](#license)  
- [REFERENCES](#references)
- [DISCLAIMER](#disclaimer)  

## OBJECTIVE  

### PURPOSE  
This project implements a chess bot in C# that connects with Stockfish engine to provide move analysis and autonomous gameplay capabilities.  

**Key Improvements:**  
- Seamless integration with Stockfish via UCI protocol  
- Console-based board visualization  
- Move validation following standard chess rules  
- Configurable engine difficulty levels  
- Real-time position evaluation  

- **Date of creation:** 2025-07  
- **Badges:**  
  ![C#](https://img.shields.io/badge/C%23-239120?logo=c-sharp&logoColor=white)  
  ![.NET](https://img.shields.io/badge/.NET-512BD4?logo=dotnet&logoColor=white)  

- **Technical:** Requires .NET 6.0 and Stockfish 16+  
- **Laboratory:** Tested on Windows 10/11 x64 systems  

### Installation Instructions  
**GitHub Repo:** [chess-bot-cs](https://github.com/Naoyuki-Christopher-H/chess-bot-cs)  

1. **Clone** the repository  
2. **Place** Stockfish binary in `/engines` folder  
3. **Run** using:  
   ```bash
   dotnet run --project ChessBot
   ```

### Key Features  
- Provides move suggestions in algebraic notation  
- Supports full autonomous gameplay  
- Displays evaluation scores in centipawns  
- Handles standard chess rules and special moves  

### File Structure  
```
chess-bot-cs/
├── ChessEngine/
│   ├── Board.cs
│   ├── Game.cs
│   ├── Move.cs
│   ├── Piece.cs
│   └── RulesValidator.cs
├── MachineLearning/
│   ├── DecisionMaker.cs
│   ├── MoveEvaluator.cs
│   └── TrainingData.cs
├── DataModels/
│   ├── GameHistory.cs
│   ├── EloRating.cs
│   └── MoveRecord.cs
├── UI/
│   ├── MainWindow.xaml
│   ├── MainWindow.xaml.cs
│   ├── ChessBoardControl.xaml
│   └── ChessBoardControl.xaml.cs
├── Utilities/
│   ├── FileLogger.cs
│   ├── PgnParser.cs
│   └── EloCalculator.cs
└── App.config

```

## LICENSE  
- **License Name:** MIT  
- **Role:** Allows modification and distribution with attribution. No warranty provided.  
- **License File:** [FULL LICENSE](LICENSE)  

## REFERENCES  

IF THIS REPOSITORY IS USED, PLEASE USE THIS TEMPLATE AS A REFERENCE:

> Author(s). (Year). *Title of Repository*. Available at: \[URL] (Accessed: \[Date]).

---

**DISCLAIMER**  

UNDER NO CIRCUMSTANCES SHOULD IMAGES OR EMOJIS BE INCLUDED DIRECTLY IN 
THE README FILE. ALL VISUAL MEDIA, INCLUDING SCREENSHOTS AND IMAGES OF 
THE APPLICATION, MUST BE STORED IN A DEDICATED FOLDER WITHIN THE PROJECT 
DIRECTORY. THIS FOLDER SHOULD BE CLEARLY STRUCTURED AND NAMED ACCORDINGLY 
TO INDICATE THAT IT CONTAINS ALL VISUAL CONTENT RELATED TO THE APPLICATION 
(FOR EXAMPLE, A FOLDER NAMED IMAGES, SCREENSHOTS, OR MEDIA).

I AM NOT LIABLE OR RESPONSIBLE FOR ANY MALFUNCTIONS, DEFECTS, OR ISSUES THAT 
MAY OCCUR AS A RESULT OF COPYING, MODIFYING, OR USING THIS SOFTWARE. IF YOU 
ENCOUNTER ANY PROBLEMS OR ERRORS, PLEASE DO NOT ATTEMPT TO FIX THEM SILENTLY 
OR OUTSIDE THE PROJECT. INSTEAD, KINDLY SUBMIT A PULL REQUEST OR OPEN AN ISSUE 
ON THE CORRESPONDING GITHUB REPOSITORY, SO THAT IT CAN BE ADDRESSED APPROPRIATELY 
BY THE MAINTAINERS OR CONTRIBUTORS.

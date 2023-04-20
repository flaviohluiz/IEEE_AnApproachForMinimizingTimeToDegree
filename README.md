# Code for implementation and analysis of the optimization model

![Resumo gráfico](https://user-images.githubusercontent.com/70773820/233487739-920a8885-f652-4093-9170-9c6d28adff5a.png)

Implementation of the mathematical model described in the article "An Approach for Minimizing Time to Degree Based on University Course Timetabling Problem", published in IEEE Latin America Transactions. The code is written in the C# programming language. The following describes the requirements and the step-by-step instructions for using the code.

## Requirements

* Visual Studio 2021 or later.
* Gurobi Optimization 9 or later (the latest version is recommended).

## Instructions for running

1. Clone the repository on your computer.
2. Open Visual Code and choose the "open a project or solution" option. Then look in the repository folder for the project and choose the "open" option.
3. Add the gurobi reference to the project. Click on "project", then "Add COM reference" and locate in the gurobi installation folder the file with the ".NET" extension.
4. Add the path to the input and output files.
* In the tab ProblemaEscala.cs:
i) line 54: subjects input file (Discentes.csv)
ii) line 72: student's input file (Disciplinas.txt)
iii) line 323: the log output file
iv) line 721: variable X output file
v) line 744: variable Y output file

* In the tab Form1.cs:
i) line 24: lp model output file
ii) line 26: solution output file

5. Click on the "run" button. A window will open and you must click on the button in it.

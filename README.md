# StudentFitnessTracker
A C# console-based Student Fitness Tracker application using JSON file handling for data persistence.

# Student Fitness Tracker

## Project Overview
The Student Fitness Tracker is a console-based application developed using C#. It helps users manage their fitness activities, monitor their workout history, maintain fitness profiles, and manage membership information.

The application uses JSON files for data storage and persistence, allowing information to be retained even after the program is closed.

## Features

- **User Authentication** – Login and account registration with password hashing and salting.
- **Membership Management** – Register, renew, and view membership information.
- **Exercise Library** – Browse exercises organized by categories and workout phases.
- **Workout Management** – Start workouts and record exercises, sets, repetitions, and weights.
- **Workout History** – View previously recorded workouts.
- **Fitness Profile** – Manage personal fitness information and goals.
- **Progress Tracking** – View fitness information and workout statistics.
- **Admin Menu** – Manage accounts, memberships, and member fitness information.

## Technologies Used

- C#
- .NET 10
- JSON
- System.IO
- System.Text.Json

## Requirements

- .NET 10 SDK
- Visual Studio or another compatible C# development environment

## Installation and Running

1. Download or clone this repository.
2. Open the solution or project file using Visual Studio.
3. Make sure the .NET 10 SDK is installed.
4. Build the project.
5. Run the application.

## Data Storage

The application stores its data in JSON files:

| File | Purpose |
|---|---|
| `users.json` | Stores user account information |
| `memberships.json` | Stores membership information |
| `exercises.json` | Stores exercise library data |
| `workouts.json` | Stores workout records |
| `profiles.json` | Stores fitness profile information |

These files allow the application to save and retrieve data between program executions.

## Project Structure

The project is organized into separate classes based on their responsibilities, including authentication, menus, exercise management, membership management, workout tracking, profile management, and file storage.

This modular structure helps improve code readability, organization, and maintainability.

## Limitations

- The application uses a console-based interface.
- Data is stored locally using JSON files instead of a database.
- Password input is visible while typing.
- Progress tracking is limited to the information and statistics implemented in the application.
- The application does not currently include graphical charts or a graphical user interface.

## Developer
Developed as a student programming project to demonstrate object-oriented programming, file handling, data persistence, validation, and basic application development using C#.

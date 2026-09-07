## WGU Capstone: Mobile App

## Description
A mobile application built with C# and .NET MAUI. This is a mobile scheduling app that helps students organize their academic progress by managing terms, courses, and assessments in one place. Users can add terms with start and end dates, assign courses to each term with instructor contact information and enrollment status, and track objective and performance assessments with their own due dates. The app includes date notifications that alert users when a course or assessment is approaching, a notes field on each course with a share option for sending notes by email or message, a search feature that finds courses across all terms, and a report screen that summarizes all academic data in a single table. All data is stored locally on the device using SQLite.

## Tech Stack
- C# and .NET MAUI
- SQLite for local data storage
- Android (API level 29+)

## Installation

**Requirements**
- An Android device or emulator running Android 10 (API level 29) or higher
- The ability to install apps from unknown sources (required for sideloaded APKs)

**Steps**
1. Download the APK from the link below.
2. On your Android device, open Settings, go to Security (or Privacy on some devices), and enable Install from Unknown Sources or Install Unknown Apps.
3. Open the downloaded APK file on your device.
4. Follow the on-screen prompts to complete installation.
5. Launch the app from your home screen or app drawer.

**Download**

[Download landing page](https://jjasher.github.io/MobileApp)

**Installing via ADB (for developers)**

If you are installing on an emulator or prefer using the command line:

```bash
adb install -r C971.apk
```

The `-r` flag allows reinstallation over an existing version.

## Project Status
Assignment was submitted and passed. Active development has stopped, though the app remains functional as a demonstration of the coursework.

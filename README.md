# Profile Card Maker & Tap Reader in C#

A **desktop application** built using **C# and .NET** that allows users to generate **profile cards** with personal information and supports **smart card tap reading**.

## Features

✅ **Profile Card Maker**  
- Generate **profile cards** with **name, designation, and profile picture**.  
- Save generated cards as **PNG, JPEG, or BMP** images.  
- **Easy-to-use** interface.
- **Read stored profile** (bytes-array or string and image).

✅ **Tap Reader (Smart Card Integration)**  
- **Detects smart cards** when tapped on the reader.  
- **Reads profile data** stored on a **Mifare 4K or 1K Smart Card**.  
- **Displays user information** upon tapping the card.  
- **Uses PC/SC (WinSCard API)** for communication.
- **Automatically compares to database (Employee/Student) and tracks tap time.**

## Prerequisites
- **.NET 8.0 or later**  
- A **PC/SC-compatible** smart card reader (e.g., **OMNIKEY CardMan 5x21**)
- A smart card (e.g., **MIFARE Classic EV1 4K**)

## Installation
1. **Clone this repository**:  
   ```sh
   git clone https://github.com/YanuarGuo/ProfileCard.git
   ```
2. **Open the project** in **Visual Studio** or any C# IDE.  
3. **Build & Run** the application.  

## Usage

### Generating a Profile Card
1. Enter your **name, bio, and upload a profile picture**.  
2. Customize the **card design or text** (optional).  
3. Click **"Confirm"** to personalize the card.  
4. Click **"Read Profile"** to read the card.  

### Using the Tap Reader
1. Connect your **PC/SC smart card reader**.  
2. Tap your **Mifare 4K Smart Card** on the reader.  
3. The application will detect the card and **read the stored profile data**.  
4. The **profile details** will be displayed automatically.  
5. Can be used as an **Employee/Student attendance system**.

## Tap Reader Implementation Methods

The tap reader supports two methods for detecting and reading smart cards:

✅ **Timer-Based Method**  
- Uses a timer to periodically check for a card presence.  
- Easier to implement and integrates well with the UI thread.  
- Suitable for applications that do not require ultra-fast detection.  

✅ **Thread-Based Method**  
- Runs in a separate thread to detect cards in real time.  
- More efficient for performance-sensitive applications.  
- Requires `BeginInvoke()` or `Invoke()` for UI updates.  

## Author
**Yanuar Christy Ade Utama**  
📧 yanuarchristyadeutama@gmail.com

## Copyright & License
© 2025 Yanuar Christy Ade Utama. All rights reserved. Unauthorized copying, modification, or distribution of this software is prohibited.


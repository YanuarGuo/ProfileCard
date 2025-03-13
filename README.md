# Profile Card Maker & Tap Reader in C#

A **desktop application** built using **C# and .NET** that allows users to generate **profile cards** with personal information and supports **smart card tap reading**.

## Features

✅ **Profile Card Maker**  
- Generate **profile cards** with **name, designation, and profile picture**.  
- **Customizable design** for profile cards.  
- Save generated cards as **PNG, JPEG, or BMP** images.  
- **Easy-to-use** interface.  

✅ **Tap Reader (Smart Card Integration)**  
- **Detects smart cards** when tapped on the reader.  
- **Reads profile data** stored on a **Mifare 4K Smart Card**.  
- **Displays user information** upon tapping the card.  
- **Uses PC/SC (WinSCard API)** for communication.  

## Prerequisites
- **.NET 8.0 or later**  
- A **PC/SC-compatible** smart card reader (e.g., **ACS ACR128U**)  

## Installation
1. **Clone this repository**:  
   ```sh
   git clone https://github.com/YanuarGuo/ProfileCard.git
   ```
2. **Open the project** in **Visual Studio** or any C# IDE.  
3. **Build & Run** the application.  

## Usage

### Generating a Profile Card
1. Enter your **name, designation, and upload a profile picture**.  
2. Customize the **card design** (optional).  
3. Click **"Generate"** to preview the card.  
4. Save the card as an **image (PNG, JPEG, BMP)**.  

### Using the Tap Reader
1. Connect your **PC/SC smart card reader**.  
2. Tap your **Mifare 4K Smart Card** on the reader.  
3. The application will detect the card and **read the stored profile data**.  
4. The **profile details** will be displayed automatically.


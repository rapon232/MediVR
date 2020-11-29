# MediVR

**Version 1.0.0**

![image](Gallery/Screenshot%202020-11-26%20at%2013.25.58.png)

A medical Virtual Reality application for exploring 3D medical datasets on the Oculus Quest.

MediVR aims to contribute to the medical field by allowing medical doctors, professors and students to import, view, explore, manipulate and export 3D DICOM studies in an immersive VR environment. The application offers a modern and intuitive user interface to interact with medical datasets in 3D space, rather than on screen.

Developed on Unity, by Dimitar Tahov as a Bachelor's Thesis under [Prof. Dr.-Ing. Kay Otto for](https://www.researchgate.net/profile/Kay_Otto) [Gesundheitselektronik](https://ge-bachelor.htw-berlin.de) course of study @ [HTW Berlin](https://www.htw-berlin.de).

![image](Gallery/Screenshot%202020-11-26%20at%2013.28.54.png)

![image](Gallery/Screenshot%202020-11-13%20at%2021.58.22.png)

![image](Gallery/Screenshot%202020-11-13%20at%2021.57.56.png)

Video of app at runtime with imported Abdomen CT Dataset available [here](https://youtu.be/WNjk1fsayIA).

Video of app at runtime with imported Brain CT Dataset available [here](https://youtu.be/NMsnznsmv3w).

---

## Instructions for Setup & Use

### Instructions for project setup

  1. Clone repo & open project in Unity.
  2. Open Build Settings & switch plattform to Android.
  3. Go to folder Resources -> MediVR -> Scenes & load scene **VirtualDiagnosticsMenu** in Hierarchy.
  4. Open Build Settings & add scene to Build (Scene index should be 0).
  5. Go to folder Resources -> MediVR -> Scenes & load scene **VirtualDiagnosticsCabinet** in Hierarchy.
  6. Open Build Settings & add scene to Build (Scene index should be 1).

### Instructions for DICOM import

For example use, download a sample CT Dataset from the [Dicom Library](https://www.dicomlibrary.com).

  1. Open Scene **VirtualDiagnosticsMenu** in Hierarchy and locate object **Dicom_Importer**.
  2. To set the path to a DICOM directory, go to Menu Bar -> MediVR -> Choose Dicom Source Directory, it should appear under the Inspector Tab.
  3. The slices should be ordered in ascending order from distal to proximal side of volume, if other way around, tick box **Reverse Slice Order**. All Slices in one directory should also be part of only one study.
  4. Tick box **Annonymize DICOM Meta Data** to annonymize data on import.
  5. Hit Button **Import Dicom Files** and wait for confirmation on console. Depending on the amount of files, this may take a couple of minutes, be patient.
  6. Button **Save Imported Directory Names** does not need to be pressed while importing, it is there only in case pre-imported data gets manually copied to project and needs to be added to directory list.
  7. Repeat process for all DICOM studies to be imported.
  8. Upload new .APK to Quest each time more data is imported into project.

### Instructions for project launch

  1. After import is complete, hit File -> Build & Run and wait for upoad of .APK file onto Quest.
  2. MediVR starts automatically on the device.

### Instructions for app use

  1. After opening of the app, choose imported study from the menu screen and the virtual cabinet automatically loads.
  ![image](Gallery/Screenshot%202020-11-10%20at%2012.03.17.png)
  2. The left wall shows pertinent information related to loaded study.
  ![image](Gallery/Screenshot%202020-11-11%20at%2013.22.46.png)
  3. The Back wall shows an instruction screen for use of the application.
  ![image](Gallery/Screenshot%202020-11-26%20at%2013.27.29.png)
  2. Hit button **Show Instructions** on the back wall and get acquainted with all joystick combinations for interaction with the loaded volume.
  3. Hit button **Show Orientation** on the back wall to show patient orientation relative to study. The orientation will appear next to the loaded volume.
  ![image](Gallery/Screenshot%202020-11-11%20at%2013.27.52.png)
  6. The 3D Volume is located in the centre of the room. A floating screen above it shows window parameters and scale as well as some buttons.
  ![image](Gallery/Screenshot%202020-11-13%20at%2021.54.52.png)
  3. To toggle on/off the cutting of the study's black background, hit **Cut Background** button on the floating screen.
  8. To save all reproduced slices, hit the **Save Images** button. They will appear in .PNG format in the MediVR/Saved Slices directory on the Quest's internal storage.
  3. To reset cutting plane position and window, hit the **↺** button.
  4. To go back to menu and quit or choose different study, hit **Open Menu** button.
  10. The right wall has 10 interactive fields for comparing slices. To pin slices to wall, drag them near to the field and release. To remove, either delete or drag outside screen.
  ![image](Gallery/Screenshot%202020-11-26%20at%2013.31.28.png)
  ![image](Gallery/Screenshot%202020-11-19%20at%2012.56.31.png)

### Instructions for project launch in Unity

  1. Make sure ONLY the **VirtualDiagnosticsMenu** scene is loaded in hierarchy.
  2. Hit Play and click on desired dataset in menu under the Game tab. Next Scene should load automatically.
  3. Slices can be duplicated using "d" button on keyboard and the window can be set from quad shader directly.
---

## License & Copyright

© Dimitar Tahov 2020

Student @ [HTW Berlin](https://www.htw-berlin.de), [Gesundheitselektronik](https://ge-bachelor.htw-berlin.de) Bachelor's thesis under [Prof. Dr.-Ing. Kay Otto](https://www.researchgate.net/profile/Kay_Otto).

MediVR licensed under the [GNU General Public License v3](LICENSE.txt).

---

## Contributors

  - Dimitar Tahov <s0560335@htw-berlin.de>

---

## Dependencies from Unity Asset Store (free)
  
  - Oculus VR Intergration by Oculus
  - fo-dicom by Cureos
  - 3D SciFi Starter Kit by Creepy Cat


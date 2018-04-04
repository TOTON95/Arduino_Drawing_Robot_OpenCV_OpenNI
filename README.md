# Drawing Robot using Arduino, serial communication with the option to draw via OpenCV/OpenNI using a kinect sensor

This project used a 3D printed robotic arm designed by Sembot (https://www.thingiverse.com/thing:1165504). 

The code of the arduino board is capable of drive the servos attached to the 3D printed parts in the desired angle calculated by the C Sharp program. 

The C Sharp program displays an blank canvas where the user can draw anything and transform the coordinates to be used by the Arduino stored in a text file.

There is an option where it can be used the Kinect sensor to draw the coordinates using the depth sensor and the color camera to draw, this is possible thanks to OpenCV and OpenNI. This program its coded in C++ and dumps the coordinates in the same text file.

There is a simulator that I used to check the functionality of the robotic arm.

_This project still in progress, constructive critiques and suggestions are welcome._ 

## Materials

* Arduino UNO
* 2 Servomotors Futaba S3003
* 1 9g Microservo 
* Base
* Screws and nuts
* Pen

## Images

<p align="center"><img src="https://raw.githubusercontent.com/TOTON95/Arduino_Drawing_Robot_OpenCV_OpenNI/master/images/Brazo_hd.png"></p>
<p align="center"><img src="https://raw.githubusercontent.com/TOTON95/Arduino_Drawing_Robot_OpenCV_OpenNI/master/images/drawing.gif"></p>
<p align="center"><img src="https://raw.githubusercontent.com/TOTON95/Arduino_Drawing_Robot_OpenCV_OpenNI/master/images/20161018_211314.jpg"></p>
<p align="center"><img src="https://raw.githubusercontent.com/TOTON95/Arduino_Drawing_Robot_OpenCV_OpenNI/master/images/20161129_081711.jpg"></p>
<p align="center"><img src="https://raw.githubusercontent.com/TOTON95/Arduino_Drawing_Robot_OpenCV_OpenNI/master/images/20161129_081729.jpg"></p>
<p align="center"><img src="https://raw.githubusercontent.com/TOTON95/Arduino_Drawing_Robot_OpenCV_OpenNI/master/images/Imagen1.jpg"></p>
<p align="center"><img src="https://raw.githubusercontent.com/TOTON95/Arduino_Drawing_Robot_OpenCV_OpenNI/master/images/ULSA_comp.png"></p>
<p align="center"><img src="https://raw.githubusercontent.com/TOTON95/Arduino_Drawing_Robot_OpenCV_OpenNI/master/images/kinect.jpg"></p>
<p align="center"><img src="https://raw.githubusercontent.com/TOTON95/Arduino_Drawing_Robot_OpenCV_OpenNI/master/images/simulator.jpg"></p>






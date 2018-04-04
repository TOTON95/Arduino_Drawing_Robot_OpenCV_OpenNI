//CREATED BY TOTON (ALEXIS GUIJARRO)@2016. FOR EDUCATIONAL PURPOSES

#include <iostream>
#include "opencv2\highgui\highgui.hpp"
#include "opencv2\imgproc\imgproc.hpp"
#include "opencv2\photo\photo.hpp"
#include "OpenNI.h"
#include <fstream>


using namespace cv;
using namespace std;
using namespace openni;

int bajo = 20;
int alto = 100;

int iLastX = -1;
int iLastY = -1;

void escribir(string,string);
int main()
{
	HWND consoleWindow = GetConsoleWindow();
	SetWindowPos(consoleWindow, 0, 750, 250, 0, 0, SWP_NOSIZE | SWP_NOZORDER);

	Status rc = STATUS_OK;
	const char* deviceURI = ANY_DEVICE;
	Device kinect;
	VideoStream color;
	VideoStream depth;

	rc = OpenNI::initialize();
	cout << "Iniciando... " << OpenNI::getExtendedError() << endl;
	rc = kinect.open(deviceURI);
	if (rc != STATUS_OK)
	{
		cout << "Fallo al abrir" << endl;
		OpenNI::shutdown();
		return 1;
	}
	rc = color.create(kinect, SENSOR_COLOR);
	if (rc == STATUS_OK)
	{
		rc = color.start();
		if (rc != STATUS_OK)
		{
			color.destroy();
			OpenNI::shutdown();
			return 2;
		}
		cout << "Color listo...";
	}
	rc = depth.create(kinect, SENSOR_DEPTH);
	if (rc == STATUS_OK)
	{
		rc = depth.start();
		if (rc != STATUS_OK)
		{
			depth.destroy();
			OpenNI::shutdown();
			return 3;
		}
		cout << "Depth listo";
	}

	FreeConsole();

	namedWindow("Control", CV_WINDOW_FREERATIO);
	resizeWindow("Control", 300, 250);
	createTrackbar("Bajo", "Control", &bajo, 200);
	createTrackbar("Alto", "Control", &alto, 200);


	Mat imgLines = Mat::zeros(Size(690, 735), CV_8UC3);
	Mat imgCircles = Mat::zeros(Size(690, 735), CV_8UC3);
	
	while (true)
	{
		
		if (!kinect.isValid())
		{
			break;
		}
		VideoFrameRef imagenColor;
		rc = color.readFrame(&imagenColor);
		if (rc == STATUS_OK)
		{
			Mat videoframecolor(imagenColor.getHeight(), imagenColor.getWidth(), CV_8UC3, (void*)imagenColor.getData());
			cvtColor(videoframecolor, videoframecolor, CV_BGR2RGB);
			resize(videoframecolor, videoframecolor, Size(1280, 960));
			//imshow("CÁMARA", videoframecolor);
			Mat roi = videoframecolor(Rect(0, 0, 690, 735));
			Mat temp;
			resize(roi, temp, Size(), 0.2, 0.2);
			GaussianBlur(temp, temp, Size(3, 3), 0.1);
			resize(temp, temp, roi.size());
			//imshow("TEMP",temp);
			/*Mat imgHSV;
			cvtColor(roi, imgHSV, COLOR_BGR2HSV);
			//imshow("HSV", imgHSV);*/
			/*Mat imagenThresholded;
			inRange(imgHSV, Scalar(iLowH, iLowS, iLowV), Scalar(iHighH, iHighS, iHighV), imagenThresholded);
			erode(imagenThresholded, imagenThresholded, getStructuringElement(MORPH_ELLIPSE, Size(5, 5)));
			dilate(imagenThresholded, imagenThresholded, getStructuringElement(MORPH_ELLIPSE, Size(5, 5)));
			dilate(imagenThresholded, imagenThresholded, getStructuringElement(MORPH_ELLIPSE, Size(5, 5)));
			erode(imagenThresholded, imagenThresholded, getStructuringElement(MORPH_ELLIPSE, Size(5, 5)));
			Moments oMoments = moments(imagenThresholded);

			double dM01 = oMoments.m01;
			double dM10 = oMoments.m10;
			double dArea = oMoments.m00;

			//25000
			//45000
			//80000
			if (dArea > 200000)
			{
				//Posicionar escritura de datos.
				int posX = dM10 / dArea;
				int posY = dM01 / dArea;

				if (iLastX >= 0 && iLastY >= 0 && posX >= 0 && posY >= 0)
				{
					string x;
					string y;
					int x1;
					int y1;
					double volteo = (posY - 695) *(-1);
					line(imgLines, Point(posX, posY), Point(iLastX, iLastY), Scalar(0, 255, 0), 2);
					x1 = posX / 7.8;
					y1 = volteo / 8.268;
					if (x1 > 0 && y1 > 0)
					{
						x = to_string(x1);
						y = to_string(y1);
						escribir(x, y);
					}
				}

				iLastX = posX;
				iLastY = posY;

			}
			imshow("Thresholded Image", imagenThresholded);*/
			//roi = roi + imgLines
			temp = temp + imgLines + imgCircles;
			imshow("TRAZADO", temp);
			//moveWindow("TRAZADO", 2040, 0);
			moveWindow("TRAZADO", 680, 0);
		}
		VideoFrameRef imagenDepth;
		rc = depth.readFrame(&imagenDepth);
		if (rc == STATUS_OK)
		{
			Mat videoframedepth(imagenDepth.getHeight(), imagenDepth.getWidth(), CV_16UC1, (void*)imagenDepth.getData());
			Mat finale(imagenDepth.getHeight(), imagenDepth.getWidth(), CV_16UC1);
			videoframedepth.convertTo(videoframedepth, CV_8UC1, 255.00/2048.00);
			//imshow("alberto", videoframedepth);
			const unsigned char noDepth = 0;
			resize(videoframedepth, videoframedepth, Size(), 0.2, 0.2);
			cv:inpaint(videoframedepth, (videoframedepth == noDepth), videoframedepth, 5.0, INPAINT_TELEA);
			GaussianBlur(videoframedepth, videoframedepth, Size(3, 3), 0.1);
			resize(videoframedepth, videoframedepth, finale.size());
			videoframedepth.convertTo(finale, CV_16UC1, 2048 / 255);
			resize(videoframedepth, videoframedepth, Size(1280, 960));
			Mat roidepth = videoframedepth(Rect(0, 0, 690, 735));
			Mat imagenThresholded;
			inRange(roidepth, (uint16_t)bajo, (uint16_t)alto, imagenThresholded);
			erode(imagenThresholded, imagenThresholded, getStructuringElement(MORPH_ELLIPSE, Size(5, 5)));
			dilate(imagenThresholded, imagenThresholded, getStructuringElement(MORPH_ELLIPSE, Size(5, 5)));
			dilate(imagenThresholded, imagenThresholded, getStructuringElement(MORPH_ELLIPSE, Size(5, 5)));
			erode(imagenThresholded, imagenThresholded, getStructuringElement(MORPH_ELLIPSE, Size(5, 5)));
			Moments oMoments = moments(imagenThresholded);

			double dM01 = oMoments.m01;
			double dM10 = oMoments.m10;
			double dArea = oMoments.m00;

			//25000
			//45000
			//80000
			if (dArea > 25000)
			{
				
				//Posicionar escritura de datos.
				int posX = dM10 / dArea;
				int posY = dM01 / dArea;

				if (iLastX >= 0 && iLastY >= 0 && posX >= 0 && posY >= 0)
				{
					string x;
					string y;
					int x1;
					int y1;
					double volteo = (posY - 695) *(-1);
					//circle(imgCircles, Point(posX, posY), 15, Scalar(0, 255, 255));
					line(imgLines, Point(posX, posY), Point(iLastX, iLastY), Scalar(0, 255, 0), 2);
					x1 = posX / 7.8;
					y1 = volteo / 8.268;
					if (x1 > 86)
					{
						x1 = 86;
					}
					if (x1 > 0 && y1 > 0)
					{
						x = to_string(x1);
						y = to_string(y1);
						escribir(x, y);
					}
				}

				iLastX = posX;
				iLastY = posY;
			}
			//show("PROFUNDIDAD", videoframedepth);
			imshow("Threshold", imagenThresholded);
		}
		if (waitKey(20) == 27)
		{
			break;
		}
	}

	color.destroy();
	depth.destroy();
	kinect.close();
	OpenNI::shutdown();
	return 0;

}
void escribir(string x, string y)
{
	ofstream outfile;
	outfile.open("path\\to\\GCODE.txt", std::ios_base::app);
	//CNC
	//string pack = string("G01 ") + string("X") + string(x) + string(" ") + string("Y") + string(y) + string(" ") + string("Z-2") + "\n";
	//ARDUINO
	string pack = string("X") + string(x) + string(" ") + string("Y") + string(y) + string(" ") + string("Z2") + "\n";
	outfile << (pack);
}

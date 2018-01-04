using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using ZedGraph;
using System.Drawing;

// program files!dotnet/sdk/nugetfallbackfolder!microsoft.netcore.app!2.0.3!ref!netcoreapp2.0!system.drawing.dll

public class VisionController : MonoBehaviour {

	[SerializeField]
	Texture2D targetTexture;
	[SerializeField]
	CanonController canon;

    VideoCapture webCam;
    VideoWriter writer;
	Mat imageOrig;
    Mat imageGray;
	Mat imageHSV;
	Mat imageResult;

	bool detectColorBlue = true;
    int imSize = 400;
    string imNameOrig = "Image";
	string imNameColor = "Color";
	string imNameResult = "Result";
    //string imAddress = "D:\\Documents_Edwyn\\vision\\drop.avi";
	//string imAddress = "C:\\Users\\Edwyn Luis\\Documents\\Lyon\\gamagora\\vision\\Wildlife.wmv";
	int imAddress = 0;

	double previousBiggestContourArea;

    // Use this for initialization
    void Start () {
        webCam = new VideoCapture(imAddress);
        //writer = new VideoWriter("D:\\Documents_Edwyn\\vision\\drop2.avi",webCam.GetCaptureProperty(CV_CAP_PROP_FPS), new Size(webCam.Width, webCam.Height), true);
		//writer = new VideoWriter("C:\\Users\\Edwyn Luis\\Documents\\Lyon\\gamagora\\vision\\result.avi", VideoWriter.Fourcc('M','P','4','2'), (int)webCam.GetCaptureProperty(CapProp.Fps), new Size(webCam.Width, webCam.Height), true);
		writer = new VideoWriter("C:\\Users\\Edwyn Luis\\Documents\\Lyon\\gamagora\\vision\\result.avi", VideoWriter.Fourcc('M','P','4','2'), 20, new Size(webCam.Width, webCam.Height), true);
        //CvInvoke.NamedWindow( imName );
        //CvInvoke.WaitKey( 0 );
        //CvInvoke.Imread;

		previousBiggestContourArea = -1;
    }
	
	// Update is called once per frame
	void Update () {
        imageOrig = webCam.QueryFrame();
		if (imageOrig != null)
        {
			imageGray = new Mat();
			imageHSV = new Mat();
			Mat imageAverage = new Mat();
			Mat imageMedian = new Mat();
			Mat imageGaussian = new Mat();
			Mat imageBilateral = new Mat();
			imageResult = new Mat();

			CvInvoke.Flip(imageOrig, imageOrig, FlipType.Horizontal);
			CvInvoke.Resize(imageOrig, imageOrig, new Size(imSize, imSize*webCam.Height/webCam.Width));

			CvInvoke.CvtColor(imageOrig, imageGray, ColorConversion.Bgr2Gray);
			CvInvoke.CvtColor(imageOrig, imageHSV, ColorConversion.Bgr2Hsv);
			// Draw Original Image
			//CvInvoke.Imshow(imNameOrig, imageOrig);
			// Draw Original Image
			CvInvoke.Imshow(imNameColor, imageHSV);

			Mat filteredImg = new Mat();
			//CvInvoke.BilateralFilter( imageHSV, filteredImg, 3, 75, 75 );
			//CvInvoke.GaussianBlur( imageHSV, filteredImg, new Size(7,7), 0 );
			CvInvoke.MedianBlur( imageHSV, filteredImg, 7 );
			Image<Hsv,System.Byte> rangeImg = filteredImg.ToImage<Hsv,System.Byte>();
			// Yellow 40-70; 0-255; 0-255
			Hsv bottomHsv = new Hsv( 18,127,127 ); // 0-179, 0-255, 0-255
			Hsv topHsv = new Hsv( 35,240,240 );
			if( detectColorBlue ){
				// BLue 230-180; 0-255; 0-255
				bottomHsv = new Hsv( 80,70,70 ); // 0-179, 0-255, 0-255
				topHsv = new Hsv( 120,250,250 );
			}
			Mat imagBW = rangeImg.InRange( bottomHsv, topHsv ).Mat;
			int elementSize = 5;
			Mat structureElement = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(elementSize,elementSize), new Point(-1,-1));
			CvInvoke.Erode( imagBW, imagBW, structureElement, new Point(-1,-1), 1, BorderType.Constant, new MCvScalar(0) );
			CvInvoke.Dilate( imagBW, imagBW, structureElement, new Point(-1,-1), 1, BorderType.Constant, new MCvScalar(0) );
			// Contours
			VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
			int biggestContourIndex = -1;
			double biggestContourArea = -1;
			Mat hierarchy = new Mat();
			CvInvoke.FindContours( imagBW, contours, hierarchy, RetrType.List, ChainApproxMethod.ChainApproxNone );
			if( contours.Size > 0 ){
				biggestContourIndex = 0;
				biggestContourArea = CvInvoke.ContourArea( contours[biggestContourIndex] );
			}
			for( int i = 1; i<contours.Size; i++ ){
				double currentArea = CvInvoke.ContourArea( contours[i] );
				if( currentArea > biggestContourArea ){
					biggestContourIndex = i;
					biggestContourArea = currentArea;
				}
			}
			if( contours.Size > 0 ){
				CvInvoke.DrawContours( imageOrig, contours, biggestContourIndex, new MCvScalar( 255,0,0 ), 5 );

				if( previousBiggestContourArea > 0 ){	// Object entering
					if( biggestContourArea > previousBiggestContourArea*1.2 ){
						// Going Foward
						//Debug.Log( "Front" );
						if(canon) canon.Shoot();
					}else if(biggestContourArea*1.2 < previousBiggestContourArea){
						// Going backward
						//Debug.Log( "Back" );
					}
				}
				previousBiggestContourArea = biggestContourArea;

				//* Centroid
				MCvMoments moment = CvInvoke.Moments( contours[biggestContourIndex] );
				int cx = (int)(moment.M10/moment.M00);
				int cy = (int)(moment.M01/moment.M00);

				VectorOfVectorOfPoint centroid = new VectorOfVectorOfPoint();
				Point[] points = new Point[1];
				points[0] = new Point( cx,cy );
				centroid.Push( new VectorOfPoint() );
				centroid[0].Push( points );
				CvInvoke.DrawContours( imageOrig, centroid, 0, new MCvScalar( 0,255,0 ), 5 );

				if( canon ) canon.setHorizontalPosition( cx/(float)imSize );
				//*/
			}
			else{
				// Object leaving
				previousBiggestContourArea = -1;
			}
			CvInvoke.Imshow("BW", imagBW);
			CvInvoke.Imshow(imNameOrig, imageOrig);

			// Filtering
			/*CvInvoke.Blur( imageHSV, imageAverage, new Size(5,5), new Point(-1,-1) );
			CvInvoke.Imshow("Average", imageAverage);
			CvInvoke.MedianBlur( imageHSV, imageMedian, 5 );
			CvInvoke.Imshow("Median", imageMedian);
			CvInvoke.GaussianBlur( imageHSV, imageGaussian, new Size(5,5), 0 );
			CvInvoke.Imshow("Gaussian", imageGaussian);
			CvInvoke.BilateralFilter( imageHSV, imageBilateral, 3, 75, 75 );
			CvInvoke.Imshow("Bilateral", imageBilateral);*/

			//CvInvoke.Imshow(imNameResult, imageResult);

			// Storing
			writer.Write(imageOrig);
        }
        else
        {
            webCam = new VideoCapture(imAddress);
        }

		if( Input.GetKeyDown(KeyCode.Escape) ){
			Application.Quit();
		}
	}

    void OnDestroy()
    {
        CvInvoke.DestroyAllWindows();
    }
}

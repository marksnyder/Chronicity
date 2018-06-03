/******************************************************************************
  SparkFun_Basic_MPL3115A2_Example.ino
  Joel Bartlett @ SparkFun Electronics
  Original Creation Date: June 30, 2015
  This sketch prints the barrometric preassure, altitude, and temperature F
  to the Seril port.

  Hardware Connections:
	This sketch was written specifically for the Photon Weather Shield,
	which connects the MPL3115A2 to the I2C bus by default.
  If you have an MPL3115A2 breakout,	use the following hardware setup:

    MPL3115A2 ------------- Photon
      GND ------------------- GND
      VCC ------------------- 3.3V (VCC)
      SCL ------------------ D1/SCL
      SDA ------------------ D0/SDA

  Development environment specifics:
  	IDE: Particle Build
  	Hardware Platform: Particle Photon
                       Particle Core

  This code is beerware; if you see me (or any other SparkFun
  employee) at the local, and you've found our code helpful,
  please buy us a round!
  Distributed as-is; no warranty is given.
*******************************************************************************/
#include "../SparkFun_MPL3115A2/SparkFun_MPL3115A2.h"
float pascals = 0;
float altf = 0;
float baroTemp = 0;

int count = 0;

MPL3115A2 baro = MPL3115A2();//create instance of MPL3115A2 barrometric sensor

//---------------------------------------------------------------
void setup()
{
    Serial.begin(9600);   // open serial over USB at 9600 baud

    //Initialize
  	while(! baro.begin()) {
          Serial.println("MPL3115A2 not found");
          delay(1000);
     }
     Serial.println("MPL3115A2 OK");
     //MPL3115A2 Settings
     //baro.setModeBarometer();//Set to Barometer Mode
     baro.setModeAltimeter();//Set to altimeter Mode
     
     baro.setOversampleRate(7); // Set Oversample to the recommended 128
     baro.enableEventFlags(); //Necessary register calls to enble temp, baro ansd alt

}
//---------------------------------------------------------------
void loop()
{
      //Get readings from sensor
      getBaro();
      //Rather than use a delay, keeping track of a counter allows the photon to
      //still take readings and do work in between printing out data.
      count++;
      //alter this number to change the amount of time between each reading
      if(count == 5)//prints roughly every 10 seconds for every 5 counts
      {
         printInfo();
         count = 0;
      }
}
//---------------------------------------------------------------
void printInfo()
{
//This function prints the weather data out to the default Serial Port

    //Take the temp reading from each sensor and average them.
    Serial.print("Baro Temp: ");
    Serial.print(baroTemp);
    Serial.print("F, ");

    Serial.print("Pressure:");
    Serial.print(pascals);
    Serial.print("Pa, ");

    Serial.print("Altitude:");
    Serial.print(altf);
    Serial.println("ft.");

}
//---------------------------------------------------------------
void getBaro()
{
  baroTemp = baro.readTempF();//get the temperature in F

  pascals = baro.readPressure();//get pressure in Pascals

  altf = baro.readAltitudeFt();//get altitude in feet
}
//---------------------------------------------------------------

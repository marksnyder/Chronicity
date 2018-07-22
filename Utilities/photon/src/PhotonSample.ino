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
#include "../lib/SparkFun_MPL3115A2/src/SparkFun_MPL3115A2.h"
#include "../lib/HTU21D/src/HTU21D.h"
float bp = 0.0;
int temp = 0;
int humid = 0;
int count = 0;
char publishString[40];

int pcelltriggerA = D5;
int pcellechoA = D4;
long pcellDurationA = 0;
long pcellDistanceA = 0;

long lastPublish = 0;

MPL3115A2 baro = MPL3115A2();//create instance of MPL3115A2 barrometric sensor
HTU21D htu = HTU21D();

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
     baro.setModeBarometer();

     baro.setOversampleRate(7); // Set Oversample to the recommended 128
     baro.enableEventFlags(); //Necessary register calls to enble temp, baro ansd alt

     pinMode(pcelltriggerA,OUTPUT);
     pinMode(pcellechoA,INPUT);

}
//---------------------------------------------------------------
void loop()
{
  if(millis() - lastPublish > 5000) //Publishes every 5000 milliseconds, or 5 seconds
  {
    // Record when you published
      lastPublish = millis();

      // ********************** weather stuff ****************************

      bp = (( baro.readPressure() /100) * 0.0295300);
      temp = ((1.8 * htu.readTemperature() ) + 32);
      humid = htu.readHumidity();


      // ********************* prox sensor stuff ******************


      digitalWrite(pcelltriggerA, LOW);  // Added this line
      delayMicroseconds(2); // Added this line
      digitalWrite(pcelltriggerA, HIGH);
      delayMicroseconds(10); // Added this line
      digitalWrite(pcelltriggerA, LOW);
      pcellDurationA = pulseIn(pcellechoA, HIGH);
      pcellDistanceA = (pcellDurationA/2) / 29.1;

      publishInfo();
   }

}
//---------------------------------------------------------------
void publishInfo()
{
    Particle.publish("temp", String(temp));
    Particle.publish("bp", String(bp));
    Particle.publish("humidity", String(humid));

    if(pcellDistanceA < 900)
    {
      Particle.publish("proxa", String(pcellDistanceA));
    }
}

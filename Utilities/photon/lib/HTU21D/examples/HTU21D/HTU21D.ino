#include "HTU21D.h"

HTU21D htu = HTU21D();

void setup()
{
	Serial.begin(9600);

	Serial.println("HTU21D test");

	while(! htu.begin()){
	    Serial.println("HTU21D not found");
	    delay(1000);
	}

	Serial.println("HTU21D OK");
}

void loop()
{
	Serial.println("===================");
	Serial.print("Hum:"); Serial.println(htu.readHumidity());
	Serial.print("Temp:"); Serial.println(htu.readTemperature());
	Serial.println();

	delay(1000);
}

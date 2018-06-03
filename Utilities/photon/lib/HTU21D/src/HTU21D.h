/*
Particle library for HTU21D Temperature / Humidity Sensor
By: Romain MP
Licence: GPL v3
*/

#ifndef _HTU21D_H
#define _HTU21D_H

#include "application.h"

#define HTDU21D_ADDRESS 0x40

// HTU21D Command codes
#define TRIGGER_TEMP_MEASURE_HOLD  0xE3
#define TRIGGER_HUMD_MEASURE_HOLD  0xE5
#define TRIGGER_TEMP_MEASURE_NOHOLD  0xF3
#define TRIGGER_HUMD_MEASURE_NOHOLD  0xF5
#define WRITE_USER_REG  0xE6
#define READ_USER_REG  0xE7
#define SOFT_RESET  0xFE

// HTU21D Resolution (see datasheet for more)
#define HTU21D_RES_11BITS 0x81 //11bits RH & 11bits Temp
#define HTU21D_RES_MAX 0x00 //12bits RH & 14bits Temp

#define CRC_POLY 0x988000 // Shifted Polynomial for CRC check

// Error codes
#define HTU21D_I2C_TIMEOUT 	998
#define HTU21D_BAD_CRC		999

class HTU21D {

public:
	// Constructor
	HTU21D();

	// Public Functions
	bool begin();
	float readHumidity();
	float readTemperature();
	void setResolution(byte resBits);
	void reset();

private:
	//Private Functions

	byte read_user_register();
	byte checkCRC(uint16_t message_from_sensor, uint8_t check_value_from_sensor);
};

#endif

## SparkFun MPL3115A2 Particle Library

Firmware library SparkFun's Photon Weather Shield and the MPL3115A2 Breakout.

About
-------------------

This is a firmware library for [SparkFun's Photon Weather Shield](https://www.sparkfun.com/products/13630).

[![Photon Battery Shield](https://cdn.sparkfun.com//assets/parts/1/1/0/1/7/13630-01a.jpg)](https://www.sparkfun.com/products/13630).

The MPL3115A2 is a MEMS pressure sensor that provides Altitude data to within 30cm (with oversampling enabled). The sensor outputs are digitized by a high resolution 24-bit ADC and transmitted over I2C, meaning it’s easy to interface with most controllers. Pressure output can be resolved with output in fractions of a Pascal, and Altitude can be resolved in fractions of a meter. The device also provides 12-bit temperature measurements in degrees Celsius.

Repository Contents
-------------------

* **/doc** - Additional documentation for the user. These files are ignored by the IDE. 
* **/firmware** - Source files for the library (.cpp, .h).
* **/firmware/examples** - Example sketches for the library (.cpp). Run these from the Particle IDE. 
* **spark.json** - General library properties for the Particel library manager. 

Example Usage
-------------------

Include the MPL3115A2 library:

	#include "SparkFunMPL3115A2.h" // Include the SparkFun MPL3115A2 library
	
Then use the `MPL3115A2` object to interact with it. Begin by initializing the IC:

	MPL3115A2 baro = MPL3115A2();//create instance of MPL3115A2 barrometric sensor

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

Then you can read various data like this:

	  baroTemp = baro.readTempF();//get the temperature in F
	
	  pascals = baro.readPressure();//get pressure in Pascals
	
	  altf = baro.readAltitudeFt();//get altitude in feet
	
Check out the example files in the [examples directory](https://github.com/sparkfun/SparkFun_MPL3115A2_Particle_Library/tree/master/firmware/examples) for more guidance.

Recommended Components
-------------------

* [Particle Photon](https://www.sparkfun.com/products/13345)
* [SparkFun Photon Weather Shield](https://www.sparkfun.com/products/13630)

License Information
-------------------

This product is _**open source**_! 

Please review the LICENSE.md file for license information. 

If you have any questions or concerns on licensing, please contact techsupport@sparkfun.com.

Distributed as-is; no warranty is given.

- Your friends at SparkFun.

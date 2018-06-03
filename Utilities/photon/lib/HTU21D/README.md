# HTU21D
A Library to add support of HTU21D temperature and humidity sensor to Particle core (Formerly Spark Core).

## Usage
Before your setup function
```C++
#include "HTU21D.h"
HTU21D htu = HTU21D();
```

In your setup function
```C++
    // HT21D sensor setup
    while(! htu.begin()){
        Serial.println("Couldnt find HTU21D");
        delay(1000);
    }
```
In your loop you can now get values like that
```C++
    temperature = htu.readTemperature();
    humidity = htu.readHumidity();
```

## Options
### Resolution
The sensor offers 2 configurations:
Maximum resolution : 12bits resolution for Relative humidity and 14bits for temperature
or 11bit resolution for both temperature and relative humidity
```C++
    // Set maximum resolution
    htu.setResolution(HTU21D_RES_MAX);

    // Set 11bits resolution
    htu.setResolution(HTU21D_RES_11BITS);
```

## Contributing

1. Fork it (https://github.com/romainmp/HTU21D/fork)
2. Create your feature branch (`git checkout -b my-new-feature`)
3. Commit your changes (`git commit -am 'Add some feature'`)
4. Push to the branch (`git push origin my-new-feature`)
5. Create a new Pull Request

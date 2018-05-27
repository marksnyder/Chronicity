

class Chronicity {


  static filterEvents = (filters) => {

    var url = 'http://localhost:64177/FilterEvents';
    url = url + "?nocache=" + (new Date()).getTime();

    filters.forEach(function(f) {
        url = url + '&expressions=' + encodeURIComponent(f);
    });

    return fetch(url);
  };

}


export default (Chronicity);

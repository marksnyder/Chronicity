

class Chronicity {


  static filterEvents = (filters) => {
    var url = 'http://ex.chronicity.io/FilterEvents';
    url = url + "?nocache=" + (new Date()).getTime();

    filters.forEach(function(f) {
        url = url + '&expressions=' + encodeURIComponent(f);
    });

    return fetch(url);
  };

  static searchEntities = (search) => {
    var url = 'http://ex.chronicity.io/SearchEntities';
    url = url + "?nocache=" + (new Date()).getTime();
    url = url + "&search=" + encodeURIComponent(search)
    return fetch(url);
  }

  static searchEventTypes = (search) => {
    var url = 'http://ex.chronicity.io/SearchEventTypes';
    url = url + "?nocache=" + (new Date()).getTime();
    url = url + "&search=" + encodeURIComponent(search)
    return fetch(url);
  }

}


export default (Chronicity);

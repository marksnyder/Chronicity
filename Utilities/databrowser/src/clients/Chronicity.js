

class Chronicity {


  static filterEvents = (filters) => {
    var url = 'http://localhost:64177/FilterEvents';
    url = url + "?nocache=" + (new Date()).getTime();

    filters.forEach(function(f) {
        url = url + '&expressions=' + encodeURIComponent(f);
    });

    return fetch(url);
  };

  static searchEntities = (search) => {
    var url = 'http://localhost:64177/SearchEntities';
    url = url + "?nocache=" + (new Date()).getTime();
    url = url + "&search=" + encodeURIComponent(search)
    return fetch(url);
  }

  static searchEventTypes = (search) => {
    var url = 'http://localhost:64177/SearchEventTypes';
    url = url + "?nocache=" + (new Date()).getTime();
    url = url + "&search=" + encodeURIComponent(search)
    return fetch(url);
  }

}


export default (Chronicity);

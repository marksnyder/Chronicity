

class Chronicity {


  static filterEvents = (expressions) => {
    var url = 'http://ex.chronicity.io/FilterEvents';
    url = url + "?nocache=" + (new Date()).getTime();

    expressions.forEach(function(f) {
        url = url + '&expressions=' + encodeURIComponent(f);
    });

    return fetch(url)
      .then(function(response) {
        return response.json();
      });
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

  static filterState = (expressions) => {
      var url = 'http://ex.chronicity.io/FilterState';
      url = url + "?nocache=" + (new Date()).getTime();

      expressions.forEach(function(f) {
          url = url + '&expressions=' + encodeURIComponent(f);
      });

      return fetch(url)
        .then(function(response) {
          return response.json();
        });
  }

}


export default (Chronicity);

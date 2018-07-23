

class Chronicity {


  static filterEvents = (expressions) => {
    var url = 'http://ex.chronicity.io/SearchEvents';
    url = url + "?nocache=" + (new Date()).getTime();

    expressions.forEach(function(f) {
        url = url + '&expressions=' + encodeURIComponent(f);
    });

    return fetch(url)
      .then(function(response) {
        return response.json();
      });
  };

  static getEntityState = (entityid,on) =>
  {
    var url = 'http://ex.chronicity.io/GetEntityState';
    url = url + "?nocache=" + (new Date()).getTime();
    url = url + "&on=" + encodeURIComponent(on)
    url = url + "&entityid=" + encodeURIComponent(entityid)
    return fetch(url)
      .then(function(response) {
        return response.json();
      });
  }

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
      var url = 'http://ex.chronicity.io/SearchState';
      url = url + "?nocache=" + (new Date()).getTime();

      expressions.forEach(function(f) {
          url = url + '&expressions=' + encodeURIComponent(f);
      });

      return fetch(url)
        .then(function(response) {
          return response.json();
        });
  }

  static searchClusters = (filterExpressions, clusterExpressions) => {
    var url = 'http://ex.chronicity.io/ClusterEvents';
    url = url + "?nocache=" + (new Date()).getTime();

    filterExpressions.forEach(function(f) {
        url = url + '&filterExpressions=' + encodeURIComponent(f);
    });

    clusterExpressions.forEach(function(f) {
        url = url + '&clusterExpressions=' + encodeURIComponent(f);
    });

    return fetch(url)
      .then(function(response) {
        return response.json();
      });
  }

}


export default (Chronicity);

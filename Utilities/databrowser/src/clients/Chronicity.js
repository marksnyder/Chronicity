

class Chronicity {


  static filterEvents = (filters) => {
    return fetch("http://localhost:64177/FilterEvents");
  };

}


export default (Chronicity);

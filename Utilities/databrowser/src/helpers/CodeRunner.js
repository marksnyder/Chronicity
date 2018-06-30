

class CodeRunner {


  static getCode = () => {
    var c = null; //window.localStorage.getItem('code');

    if(c == null || c == '') {

    CodeRunner.setCode(`

    var client = this.getClient();
    var that = this;

    this.clearEvents();
    this.clearStateChanges();

    client.filterEvents([
        'On.After=6/01/2018 11:00', 'Type=Bird Arrived'])
        .then(function(data){
            that.addEvents(data, { "backgroundColor": "#E57373" }, "A");
    });

    client.filterEvents([
        'On.After=6/01/2018 11:00', 'Type=Bird Departed'])
        .then(function(data){
            that.addEvents(data, { "backgroundColor": "#BA68C8" }, "D");
    });

    client.filterState([
        'On.After=6/01/2018 11:00', 'Entity.State.tempup=True'])
        .then(function(data){
            that.addStateChanges(data, 'temp','red');
    });`);
    }

    return window.localStorage.getItem('code');

  };

  static setCode = (code) => {
    window.localStorage.setItem('code',code);
  };

  static runCode = (target) => {
    try {
      CodeRunner.evalCode.call(target);
    } catch(e) {
      alert(e.message);
    }
  };

  static evalCode = () => {
      eval(CodeRunner.getCode());
  };

}

export default (CodeRunner);

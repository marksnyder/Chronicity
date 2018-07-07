

class CodeRunner {


 getCode = () => {
    return this.myCode;
  };


 loadCode = () => {
   this.myCode = window.localStorage.getItem('mycode');

   if(this.myCode == null || this.myCode == '') {

     this.myCode = `

     var chronicity = this.getClient();
      var helper = this.getUtilities();

      this.myEvents = [];
      this.myStateChanges = [];


      var arrivals = chronicity.filterEvents([
          'On.After=7/3/2018 11:00', 'Type=Bird Arrived'])
          .then((data) => {
              this.myEvents = helper.mergeEvents(this.myEvents,data,{ "backgroundColor": "#E57373" },"A");
      });

      var departures = chronicity.filterEvents([
          'On.After=7/3/2018 11:00', 'Type=Bird Departed'])
          .then((data) => {
              this.myEvents = helper.mergeEvents(this.myEvents,data,{ "backgroundColor": "#BA68C8" },"D");
      });

      var boiling = chronicity.filterState([
          'On.After=7/3/2018 11:00', 'Entity.State.temp >= 90'])
          .then((data) => {
              this.myStateChanges = helper.mergeStateChanges(this.myStateChanges,data,'temp','#ffeb3b');
      });

      var hot = chronicity.filterState([
          'On.After=7/3/2018 11:00', 'Entity.State.temp >= 80', 'Entity.State.temp < 90' ])
          .then((data) => {
              this.myStateChanges = helper.mergeStateChanges(this.myStateChanges,data,'temp','#bf360c');
      });

      var cool = chronicity.filterState([
          'On.After=7/3/2018 11:00', 'Entity.State.temp >= 70', 'Entity.State.temp < 80' ])
          .then((data) => {
              this.myStateChanges = helper.mergeStateChanges(this.myStateChanges,data,'temp','#7e57c2');
      });

      var cold = chronicity.filterState([
          'On.After=7/3/2018 11:00', 'Entity.State.temp < 70' ])
          .then((data) => {
              this.myStateChanges = helper.mergeStateChanges(this.myStateChanges,data,'temp','#42a5f5');
      });


     Promise.all([arrivals, departures, boiling, hot, cool, cold]).then(() => {
         this.setStateChanges(this.myStateChanges);
         this.setEvents(this.myEvents);
         alert('Success! Loaded ' + this.myEvents.length + ' events ' );
         console.log(this);
     }).catch(reason => {
       alert(reason.message);
       console.log(reason);
     });

     `;

    }

  }

 saveCode = () => {
    window.localStorage.setItem('mycode',this.myCode);
  }

 setCode = (code) => {
     this.myCode = code;
  }

 runCode = (target, code) => {
    try {
      this.evalCode.call(target);
    } catch(e) {
      alert(e.message);
      console.log(e);
    }
  };

 evalCode = () => {
    try {
      eval(this.myCode);
    } catch(e) {
      alert(e.message);
      console.log(e);
    }
  };

}

export default (CodeRunner);

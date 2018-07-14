import Chronicity from '../helpers/Chronicity.js'
import DataUtilities from '../helpers/DataUtilities.js'
import P from 'bluebird';

class BirdActivity {


    runExample = (target) => {

       this.myEvents = [];
       this.myStateChanges = [];
       this.calls = []

       this.calls.push(
         Chronicity.filterEvents([
           'On.After=7/3/2018 11:00',
           'Type=Bird Arrived'
         ])
           .then((data) => {
               this.myEvents = DataUtilities.mergeEvents(this.myEvents,data,{ "backgroundColor": "#E57373" },"A");
       }));

      this.calls.push(
        Chronicity.filterEvents([
           'On.After=7/3/2018 11:00',
           'Type=Bird Departed'
         ])
           .then((data) => {
               this.myEvents = DataUtilities.mergeEvents(this.myEvents,data,{ "backgroundColor": "#BA68C8" },"D");
       }));

      this.calls.push(
        Chronicity.filterState([
           'On.After=7/3/2018 11:00',
           'Entity.State.temp >= 90'
         ])
           .then((data) => {
               this.myStateChanges = DataUtilities.mergeStateChanges(this.myStateChanges,data,'temp','#f57f17');
       }));

      this.calls.push(
        Chronicity.filterState([
           'On.After=7/3/2018 11:00',
           'Entity.State.temp >= 80',
           'Entity.State.temp < 90'
          ])
           .then((data) => {
               this.myStateChanges = DataUtilities.mergeStateChanges(this.myStateChanges,data,'temp','#fbc02d');
       }));

      this.calls.push(
       Chronicity.filterState([
           'On.After=7/3/2018 11:00',
           'Entity.State.temp >= 70',
           'Entity.State.temp < 80'
         ])
           .then((data) => {
               this.myStateChanges = DataUtilities.mergeStateChanges(   this.myStateChanges,data,'temp','#4fc3f7');
       }));

      this.calls.push(
        Chronicity.filterState([
           'On.After=7/3/2018 11:00',
           'Entity.State.temp < 70' ])
           .then((data) => {
               this.myStateChanges = DataUtilities.mergeStateChanges(this.myStateChanges,data,'temp','#01579b');
       }));


      P.all(this.calls).then(() => {
              target.setStream(this.myEvents,this.myStateChanges);
          }).catch(reason => {
            alert(reason.message);
            console.log(reason);
          });

    }

}

export default (BirdActivity);

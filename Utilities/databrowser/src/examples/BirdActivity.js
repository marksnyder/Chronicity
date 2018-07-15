import Chronicity from '../helpers/Chronicity.js'
import DataUtilities from '../helpers/DataUtilities.js'
import P from 'bluebird';
import moment from 'moment';

class BirdActivity {


    runExample = (target) => {

       this.myEvents = [];
       this.myStateChanges = [];
       this.calls = []

       var begin =  (item) => {
             return  {
                 title: 'Bird Session Begin',
                 subtitle: 'Started ' + moment(item.start).format('mm:ss'),
                 iconStyle: { "backgroundColor": "#BA68C8" },
                 iconContent: "S",
                 on: item.start,
                 id: item.id,
                 entities: item.entities
             }
         };


         var end =  (item) => {
               return  {
                   title: 'Bird Session End',
                   subtitle: 'Ended ' + moment(item.end).format('mm:ss') + ' Events: ' + item.events.length + ' Duration: ' + moment(item.end).diff(moment(item.start), 'seconds') + ' seconds' ,
                   iconStyle: { "backgroundColor": "#BA68C8" },
                   iconContent: "E",
                   on: item.end,
                   id: item.id,
                   entities: item.entities
               }
           };


       this.calls.push(
         Chronicity.searchClusters(['On.After=7/3/2018 11:00'],['TimeSpan <= 0.0:20:0'])
           .then((data) => {
             this.myEvents = DataUtilities.mergeMarkers(this.myEvents,data,begin);
             this.myEvents = DataUtilities.mergeMarkers(this.myEvents,data,end);
           })
       );


      this.calls.push(
        Chronicity.filterState([
           'On.After=7/3/2018 11:00',
           'Entity.State.temp >= 90'
         ])
           .then((data) => {
               this.myStateChanges = DataUtilities.mergeTrackerChanges(this.myStateChanges,data,'temp','#f57f17');
       }));


      this.calls.push(
        Chronicity.filterState([
           'On.After=7/3/2018 11:00',
           'Entity.State.temp >= 80',
           'Entity.State.temp < 90'
          ])
           .then((data) => {
               this.myStateChanges = DataUtilities.mergeTrackerChanges(this.myStateChanges,data,'temp','#fbc02d');
       }));

      this.calls.push(
       Chronicity.filterState([
           'On.After=7/3/2018 11:00',
           'Entity.State.temp >= 70',
           'Entity.State.temp < 80'
         ])
           .then((data) => {
               this.myStateChanges = DataUtilities.mergeTrackerChanges(this.myStateChanges,data,'temp','#4fc3f7');
       }));

      this.calls.push(
        Chronicity.filterState([
           'On.After=7/3/2018 11:00',
           'Entity.State.temp < 70' ])
           .then((data) => {
               this.myStateChanges = DataUtilities.mergeTrackerChanges(this.myStateChanges,data,'temp','#01579b');
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

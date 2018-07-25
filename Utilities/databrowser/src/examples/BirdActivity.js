import Chronicity from '../helpers/Chronicity.js'
import DataUtilities from '../helpers/DataUtilities.js'
import P from 'bluebird';
import moment from 'moment';

class BirdActivity {


    runExample = (target) => {

       this.myEvents = [];
       this.myStateChanges = [];
       this.calls = []
       this.startExpression = 'On.After=' + moment().subtract(4, 'days').format();
       this.endExpression = 'On.Before=' + moment().format();

       var sessionStart =  (item) => {
             return  {
                 title: 'Bird Session Begin',
                 subtitle: 'Started ' + moment(item.start).format('hh:mm:ss'),
                 iconStyle: { "backgroundColor": "#1b5e20" },
                 iconContent: "B",
                 on: item.start,
                 id: item.id,
                 entities: item.entities
             }
         };


        var sessionEnd =  (item) => {
               return  {
                   title: 'Bird Session End',
                   subtitle: 'Ended ' + moment(item.end).format('hh:mm:ss') + ' Events: ' + item.events.length + ' Duration: ' + moment(item.end).diff(moment(item.start), 'seconds') + ' seconds' ,
                   iconStyle: { "backgroundColor": "#b71c1c" },
                   iconContent: "E",
                   on: item.end,
                   id: item.id,
                   entities: item.entities
               }
           };

         var sunset =  (item) => {
                return  {
                    title: 'Sunset',
                    subtitle:  moment(item.on).format('hh:mm:ss'),
                    iconStyle: { "backgroundColor": "#ff5722" },
                    iconContent: "S",
                    on: item.on,
                    id: item.id,
                    entities: item.entities
                }
            };


          var sunrise =  (item) => {
                 return  {
                     title: 'Sunrise',
                     subtitle:  moment(item.on).format('hh:mm:ss'),
                     iconStyle: { "backgroundColor": "#ffeb3b" },
                     iconContent: "S",
                     on: item.on,
                     id: item.id,
                     entities: item.entities
                 }
             };

         var refillStart =  (item) => {
                return  {
                    title: 'Refill Start',
                    subtitle:  moment(item.start).format('hh:mm:ss'),
                    iconStyle: { "backgroundColor": "#009faf" },
                    iconContent: "RS",
                    on: item.start,
                    id: item.id,
                    entities: item.entities
                }
            };

        var refillEnd =  (item) => {
               return  {
                   title: 'Refill End',
                   subtitle:  moment(item.end).format('hh:mm:ss'),
                   iconStyle: { "backgroundColor": "#88ffff" },
                   iconContent: "RE",
                   on: item.end,
                   id: item.id,
                   entities: item.entities
               }
           };

       this.calls.push(
         Chronicity.searchClusters([this.startExpression, this.endExpression, 'Type=MotionStart'],['Within <= 0.0:01:00'])
           .then((data) => {
             this.myEvents = DataUtilities.mergeMarkers(this.myEvents,data,sessionStart);
             this.myEvents = DataUtilities.mergeMarkers(this.myEvents,data,sessionEnd);
           })
       );


       this.calls.push(
         Chronicity.filterEvents([this.startExpression, this.endExpression, 'Type=Sunrise'])
           .then((data) => {
             this.myEvents = DataUtilities.mergeMarkers(this.myEvents,data,sunrise);
           })
       );


       this.calls.push(
         Chronicity.filterEvents([this.startExpression, this.endExpression, 'Type=Sunset'])
           .then((data) => {
             this.myEvents = DataUtilities.mergeMarkers(this.myEvents,data,sunset);
           })
       );


       this.calls.push(
         Chronicity.searchClusters([this.startExpression, this.endExpression, 'Type=Feeder Refill'],['Within <= 0.0:01:00'])
           .then((data) => {
             this.myEvents = DataUtilities.mergeMarkers(this.myEvents,data,refillStart);
             this.myEvents = DataUtilities.mergeMarkers(this.myEvents,data,refillEnd);
           })
       );


      this.calls.push(
        Chronicity.filterState([
           this.startExpression,
           this.endExpression,
           'Entity.State.temp >= 90'
         ])
           .then((data) => {
               this.myStateChanges = DataUtilities.mergeTrackerChanges(this.myStateChanges,data,'temp','#f57f17');
       }));


      this.calls.push(
        Chronicity.filterState([
           this.startExpression,
           this.endExpression,
           'Entity.State.temp >= 80',
           'Entity.State.temp < 90'
          ])
           .then((data) => {
               this.myStateChanges = DataUtilities.mergeTrackerChanges(this.myStateChanges,data,'temp','#fbc02d');
       }));

      this.calls.push(
       Chronicity.filterState([
           this.startExpression,
           this.endExpression,
           'Entity.State.temp >= 70',
           'Entity.State.temp < 80'
         ])
           .then((data) => {
               this.myStateChanges = DataUtilities.mergeTrackerChanges(this.myStateChanges,data,'temp','#4fc3f7');
       }));

      this.calls.push(
        Chronicity.filterState([
           this.startExpression,
           this.endExpression,
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

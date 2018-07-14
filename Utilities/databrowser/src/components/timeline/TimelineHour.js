import React from 'react';
import Timeline10Minutes from './Timeline10Minutes.js'
import DataUtilities from '../../helpers/DataUtilities.js'
import moment from 'moment';

class TimelineHour extends React.Component {


  render() {

    var end = this.props.group.end;

    if(moment(end).isAfter(moment()))
    {
      end = moment();
    }

    var groups = DataUtilities.groupBy10Minutes(this.props.group.events,this.props.group.stateChanges,this.props.group.start,end);

    return (<div>
      {groups.map(n => {
        return (<Timeline10Minutes key={n.id}
          viewState={this.props.viewState}
          events={n.events}
          stateChanges={n.stateChanges}
          description={n.description} />
        );
      })}
      </div>
    );
  }
}



export default  (TimelineHour);

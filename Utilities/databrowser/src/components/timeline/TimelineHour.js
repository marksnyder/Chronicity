import React from 'react';
import { withStyles } from '@material-ui/core/styles';
import Timeline10Minutes from './Timeline10Minutes.js'
import Grid from '@material-ui/core/Grid';
import DataUtilities from '../../helpers/DataUtilities.js'
import moment from 'moment';

class TimelineHour extends React.Component {

  constructor(props) {
    super(props);
  }

  render() {

    const { classes } = this.props;

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

import React from 'react';
import { withStyles } from '@material-ui/core/styles';
import TimelineHour from './TimelineHour.js'
import Grid from '@material-ui/core/Grid';
import DataUtilities from '../../helpers/DataUtilities.js'

class TimelineGroup extends React.Component {

  constructor(props) {
    super(props);
  }

  render() {

    const { classes } = this.props;

    var groups = DataUtilities.groupByHour(this.props.group.events,this.props.group.stateChanges,this.props.group.start,this.props.group.end);

    return (<div>
      {groups.map(n => {
        return (<TimelineHour key={n.id}
          viewState={this.props.viewState}
          group={n} />
        );
      })}
      </div>
    );
  }
}



export default  (TimelineGroup);

import React from 'react';
import Timeline from './Timeline.js'
import moment from 'moment';
import CircularProgress from '@material-ui/core/CircularProgress';
import { withStyles } from '@material-ui/core/styles';


const styles = theme => ({
  progress: {
    margin: theme.spacing.unit * 2,
  },
});

class TimelineView extends React.Component {


  render() {
    const { classes } = this.props;
    var start = moment().startOf('hour');
    var end = moment().subtract(3, 'days');

    if(this.props.events == null || this.props.events.length < 1)
    {
      return (<div className={classes.root} style={{ textAlign: 'center' }}>
        <CircularProgress className={classes.progress} size={100} color="secondary" />
      </div>);
    }

    return  <div className={classes.root}>
          <Timeline
            events={this.props.events}
            stateChanges={this.props.stateChanges}
            viewState={this.openStateViewer}
            start={start}
            end={end}   />
      </div>

  }
}

export default withStyles(styles)(TimelineView);

import React from 'react';
import AppBar from '@material-ui/core/AppBar';
import Tabs from '@material-ui/core/Tabs';
import Tab from '@material-ui/core/Tab';
import Typography from '@material-ui/core/Typography';
import Timeline from './Timeline.js'
import Chronicity from '../../helpers/Chronicity.js'
import moment from 'moment';
import CircularProgress from '@material-ui/core/CircularProgress';
import { withStyles } from '@material-ui/core/styles';

function TabContainer(props) {
  return (
    <Typography component="div" style={{ padding: 8 * 3 }}>
      {props.children}
    </Typography>
  );
}

const styles = theme => ({
  progress: {
    margin: theme.spacing.unit * 2,
  },
});

class TimelineView extends React.Component {

  constructor(props) {
    super(props);
  }


  render() {
    const { classes } = this.props;

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
            start={moment().startOf('hour')}
            end={moment().subtract(3, 'days')}
            classes={classes}  />
      </div>

  }
}

export default withStyles(styles)(TimelineView);

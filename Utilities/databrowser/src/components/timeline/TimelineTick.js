import React from 'react';
import { withStyles } from '@material-ui/core/styles';
import TimelineEvents from './TimelineEvents.js'
import Grid from '@material-ui/core/Grid';

class TimelineTick extends React.Component {

  constructor(props) {
    super(props);
  }

  render() {

    const { classes } = this.props;

    return (<div className='tick'>
      <Grid container>
        <Grid xs={6} sm={1}>
          <div>{this.props.description}</div>
        </Grid>
        <Grid xs={6} sm={1}>

        </Grid>
        <Grid item xs={12} sm={10}>
          <TimelineEvents
            viewState={this.props.viewState}
            events={this.props.events} />
        </Grid>
      </Grid>
    </div>);
  }
}



export default  (TimelineTick);

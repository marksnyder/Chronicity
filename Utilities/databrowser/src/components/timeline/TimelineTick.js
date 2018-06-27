import React from 'react';
import { withStyles } from '@material-ui/core/styles';
import TimelineEvents from './TimelineEvents.js'
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';

class TimelineTick extends React.Component {

  constructor(props) {
    super(props);
  }

  render() {

    const { classes } = this.props;
    //const stateKeys = ['nuthoing'];// Object.keys(this.props.stateChanges);

    return (<div className='tick'>
      <Grid container>
        <Grid xs={2} sm={2}>
          <Typography>{this.props.description}</Typography>
        </Grid>
        <Grid xs={1} sm={1}>
          <div style={{
            width: '10px',
            height: '100%',
            'background-color': '#038387',
            'float' : 'left',
            'border-left' : 'solid 1px white' }}>
          </div>
          <div style={{
            width: '10px',
            height: '100%',
            'background-color': '#E3008C',
            'float' : 'left',
            'border-left' : 'solid 1px white' }}>
          </div>
        </Grid>
        <Grid item xs={6} sm={6}>
          <TimelineEvents
            viewState={this.props.viewState}
            events={this.props.events} />
        </Grid>
      </Grid>
    </div>);
  }
}



export default  (TimelineTick);

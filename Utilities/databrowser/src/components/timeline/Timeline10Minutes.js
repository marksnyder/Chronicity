import React from 'react';
import { withStyles } from '@material-ui/core/styles';
import TimelineEvents from './TimelineEvents.js'
import Grid from '@material-ui/core/Grid';
import Typography from '@material-ui/core/Typography';

const styles = {
    'timeTickContainer' :
    {
      'backgroundColor': '#2e3336'
    },
    'timeTickTime' :
    {
      'color' : 'white',
      'paddingLeft' : '5px'
    },
    'tick' :
    {
      'backgroundColor' : '#eee'
    }
  };

class Timeline10Minutes extends React.Component {

  render() {

    var stateKeys = Object.keys(this.props.stateChanges);

    var stateItems = [];

    stateKeys.forEach(function(k) {
       var changes = this.props.stateChanges[k];
       var background = 'white';
       var desc = k + ': N/A';

       if(changes.length > 1)
       {
         background = 'repeating-linear-gradient(to bottom,' + changes[0].color + ',' + changes[0].color + ' 50%,' + changes[changes.length -1].color  +' 50%,' + changes[ changes.length -1].color + ' 20px)';
         desc = k + ': ' + changes[0].value + ' - ' +  changes[changes.length -1].value;
       }
       else if(changes.length === 1)
       {
         background = changes[0].color;
         desc = k + ': ' + changes[0].value;
       }

       stateItems.push({ id: k , desc: desc ,
         style: {
           'width': '10px',
           'height' : '100%',
           'background': background,
           'float' : 'left',
           'borderLeft' : 'solid 1px white' }
         });

    }, this);

    return (<div style={styles.tick}>
      <Grid container>
        <Grid item xs={2} sm={2} style={styles.timeTickContainer}>
          <Typography style={styles.timeTickTime}>{this.props.description}</Typography>
        </Grid>
        <Grid item xs={1} sm={1}>
            {stateItems.map(n => {
              return (<div key={n.id} style={n.style}></div>)
            })}
        </Grid>
        <Grid item xs={6} sm={6}>
          <TimelineEvents
            events={this.props.events} />
        </Grid>
      </Grid>
    </div>);
  }
}



export default  withStyles(styles)(Timeline10Minutes);

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

  constructor(props) {
    super(props);
  }

  render() {

    const { classes } = this.props;
    var stateKeys = Object.keys(this.props.stateChanges);

    var stateItems = [];

    stateKeys.forEach(function(k) {
       var changes = this.props.stateChanges[k];

       if(changes.length == 0)
       {
         stateItems.push({ id: k ,
           style: {
             'width': '10px',
             'height' : '100%',
             'backgroundColor': 'white',
             'float' : 'left',
             'borderLeft' : 'solid 1px white'
           }
          });
       }
       else if(changes.length > 1)
       {
         stateItems.push({ id: k ,
           style: {
             'width': '10px',
             'height' : '100%',
             'background': 'repeating-linear-gradient(45deg,' + changes[1].color + ' 5px,' + changes[0].color + ' 5px)',
             'float' : 'left',
             'borderLeft' : 'solid 1px white'
           }
          });
       }
       else
       {
         stateItems.push({ id: k ,
           style: {
             'width': '10px',
             'height' : '100%',
             'backgroundColor': changes[0].color,
             'float' : 'left',
             'borderLeft' : 'solid 1px white'
           }
          });
       }

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

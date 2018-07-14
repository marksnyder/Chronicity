import React from 'react';
import Typography from '@material-ui/core/Typography';
import { withStyles } from '@material-ui/core/styles';
import Button from '@material-ui/core/Button';
import TimelineDay from './TimelineDay.js'
import { Sticky, StickyContainer } from 'react-sticky';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import DataUtilities from '../../helpers/DataUtilities.js'


const styles = theme => ({
  stickyHeader: {
      'zIndex': '100',
      'opacity': '.9'
  }
});


class Timeline extends React.Component {

  constructor(props) {
    super(props);
    this.state = {
      groups: DataUtilities.groupByDay(this.props.events, this.props.stateChanges),
      loaded: 1
    };
  }

  loadMore = () => {
    this.setState({
      loaded: this.state.loaded + 1
    });
  }

  clickItem = (props) => {

    if(props.item !== undefined)
    {
      var match = this.props.events.find(function(e) {
        return e.id === props.item;
      });

      if(match !== undefined)
      {
        this.props.viewState(match);
      }
    }

  }


  render() {

    const { classes } = this.props;
    const loadedGroups = this.state.groups.slice(0,this.state.loaded);

    return (<div>
      {loadedGroups.map(n => {
        return (<div key={n.id}>
          <StickyContainer  className="container">
            <Sticky>
              {({ style }) => (
                <div style={style} className={classes.stickyHeader}>
                  <AppBar position="static" color="secondary">
                    <Toolbar>
                      <Typography variant="title" color="inherit">
                        {n.description}
                      </Typography>
                    </Toolbar>
                  </AppBar>
                </div>
              )}
            </Sticky>
          <TimelineDay viewState={this.props.viewState} group={n}  />
          </StickyContainer>
        </div>
        );
      })}
       {this.state.loaded < this.state.groups.length &&
         <div style={{ padding: '10px' }}>
         <Button onClick={() => this.loadMore()} fullWidth={true} variant="outlined" size="large" color="secondary" className={classes.button}>
                Load More...
         </Button>
       </div>
       }
      </div>
    );
  }
}



export default withStyles(styles)(Timeline);

import React from 'react';
import PropTypes from 'prop-types';
import DialogTitle from '@material-ui/core/DialogTitle';
import moment from 'moment';
import Chip from '@material-ui/core/Chip';
import Typography from '@material-ui/core/Typography';
import Avatar from '@material-ui/core/Avatar';
import Paper from '@material-ui/core/Paper';
import { withStyles } from '@material-ui/core/styles';
import Button from '@material-ui/core/Button';
import TimelineDay from './TimelineDay.js'
import { Sticky, StickyContainer } from 'react-sticky';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import DataUtilities from '../../helpers/DataUtilities.js'


var headerStyle = {
  'zIndex': '100'
};


class Timeline extends React.Component {

  constructor(props) {
    super(props);
  }

  clickItem = (props) => {

    if(props.item != undefined)
    {
      var match = this.props.events.find(function(e) {
        return e.id == props.item;
      });

      if(match != undefined)
      {
        this.props.viewState(match);
      }
    }

  }


  render() {

    const { classes } = this.props;
    if(this.props.events.length == 0) return (<div></div>);
    var groups = DataUtilities.groupByDay(this.props.events, this.props.stateChanges);
    if(groups.length == 0) return (<div></div>);

    return (<div>
      {groups.map(n => {
        return (<div key={n.id}>
          <StickyContainer  className="container">
            <Sticky>
              {({ style }) => (
                <div style={Object.assign(style,headerStyle)}>
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
      </div>
    );
  }
}



export default (Timeline);

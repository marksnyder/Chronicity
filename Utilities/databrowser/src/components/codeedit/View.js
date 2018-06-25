import React from 'react';
import { render } from 'react-dom';
import Divider from '@material-ui/core/Divider';
import Paper from '@material-ui/core/Paper';
import Grid from '@material-ui/core/Grid';
import CodeEditor from './CodeEditor.js';
import EventTypeList from './EventTypeList.js';
import EntityList from './EntityList.js';

import ListSubheader from '@material-ui/core/ListSubheader';
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import ListItemText from '@material-ui/core/ListItemText';
import Typography from '@material-ui/core/Typography';

class CodeView extends React.Component {

  constructor(props) {
    super(props);
    this.state = {
      value: 0
    };
  }

  render() {
    const { classes } = this.props;
    const { value } = this.state;

    return <div className={classes.root}>
      <Typography component="div">
        <CodeEditor
          addEvents={this.props.addEvents}
          clearEvents={this.props.clearEvents}
          addStateChanges={this.props.addStateChanges}
          clearStateChanges={this.props.clearStateChanges}
          classes={classes}  />
    </Typography>
    </div>
  }
}

export default (CodeView);

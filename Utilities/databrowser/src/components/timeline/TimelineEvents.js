import React from 'react';
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemSecondaryAction from '@material-ui/core/ListItemSecondaryAction';
import ListItemText from '@material-ui/core/ListItemText';
import Checkbox from '@material-ui/core/Checkbox';
import IconButton from '@material-ui/core/IconButton';
import CommentIcon from '@material-ui/icons/Comment';
import BluetoothIcon from '@material-ui/icons/Bluetooth';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import deepOrange from '@material-ui/core/colors/deepOrange';
import deepPurple from '@material-ui/core/colors/deepPurple';
import Avatar from '@material-ui/core/Avatar';
import { withStyles } from '@material-ui/core/styles';

const styles = {
  avatar: {
    margin: 10,
  },
  orangeAvatar: {
    margin: 10,
    color: '#fff',
    backgroundColor: deepOrange[500],
  },
  purpleAvatar: {
    margin: 10,
    color: '#fff',
    backgroundColor: deepPurple[500],
  },
  row: {
    display: 'flex',
    justifyContent: 'center',
  },
};

class TimelineEvents extends React.Component {

  constructor(props) {
    super(props);
  }

  render() {

    const { classes } = this.props;

    return (<div>
        <List>
          {this.props.events.reverse().map(n => {
            return (<ListItem
              key={n.id}
              role={undefined}
              dense
              button
              onClick={() => this.props.viewState(n)}
            >
            <ListItemIcon>
             <Avatar style={n.iconStyle}>{n.initials}</Avatar>
            </ListItemIcon>
            <ListItemText primary={n.type} />
            </ListItem>);
           })}
       </List>
   </div>);
  }
}



export default  withStyles(styles)(TimelineEvents);

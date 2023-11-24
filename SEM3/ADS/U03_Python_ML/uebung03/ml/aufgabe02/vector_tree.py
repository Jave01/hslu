
# HSLU / ICS/AIML : Modul ADS : Algorithmen & Datenstrukturen
# Path   : uebung03/ml/aufgabe02
# Version: Sun Oct  1 20:02:09 CEST 2023

import enum
from uebung03.ml.aufgabe02.no_such_node_exception import NoSuchNodeException


ROOT_INDEX = 1

class VectorTree:
  
  class _child_side(enum.Enum):
    LEFT = enum.auto()
    RIGHT = enum.auto()

  def __init__(self):
    self._binary_tree = []
    self._binary_tree.append(None)
    self._binary_tree.append(None)
    self._size = 0
    
  def root(self):
    return self._binary_tree[ROOT_INDEX]
  
  def set_root(self, root):
    if self.root() != None:
      self._remove(ROOT_INDEX)
    self._binary_tree[ROOT_INDEX] = root
    self._size = 1

  def parent(self, child):
    if self.is_root(child):
      return None  # this object is the root... thus no parent...
    return self._binary_tree[self._position(child) // 2]

  def left_child(self, parent):
    return self._get_child(parent, VectorTree._child_side.LEFT)

  def right_child(self, parent):
    return self._get_child(parent, VectorTree._child_side.RIGHT)

  def _get_child(self, parent, child_side): 
    child_pos = self._child_pos_by_value(parent, child_side)
    if not self._has_node_at_position(child_pos):
      return None
    return self._binary_tree[child_pos]

  def _child_pos_by_value(self, parent, child_side):
    return self._child_pos_by_index(self._position(parent), child_side)
  
      
  def _child_pos_by_index(self, parent_pos, child_side):
    return parent_pos * 2 + (0 if child_side == VectorTree._child_side.LEFT else 1)

  def is_internal(self, node):
    return not self.is_external(node)

  def is_external(self, node):
    left_child_pos = self._child_pos_by_value(node, VectorTree._child_side.LEFT)
    rigth_child_pos = left_child_pos + 1
    result = None
    if self._has_node_at_position(left_child_pos) or self._has_node_at_position(rigth_child_pos):
      return False
    else: 
      return True
    return result
  
  
  def _has_node_at_position(self, pos):
    if pos > (len(self._binary_tree) - 1):
      return False
    return self._binary_tree[pos] != None

  def is_root(self, node):
    return node == self._binary_tree[ROOT_INDEX]

  def set_right_child(self, parent, child):
    self._set_child(parent, child, VectorTree._child_side.RIGHT)

  def set_left_child(self, parent, child):
    self._set_child(parent, child, VectorTree._child_side.LEFT)

  def _set_child(self, parent, child, child_side):
    self._remove_child(parent, child_side)
    child_pos = self._child_pos_by_value(parent, child_side)
    self._assure_size(child_pos)
    if self._binary_tree[child_pos] == None:
      self._size += 1
    self._binary_tree[child_pos] = child 
      
  def remove_right_child(self, parent):
    self._remove_child(parent, VectorTree._child_side.RIGHT)

  def remove_left_child(self, parent):
    self._remove_child(parent, VectorTree._child_side.LEFT)
      
  def _remove_child(self, parent, child_side):
    child_pos = self._child_pos_by_value(parent, child_side)
    if self._has_node_at_position(child_pos): 
      self._remove(child_pos)

  def _remove(self, node_pos):
    """ Precondition: Node exists. """
    for child_side in VectorTree._child_side:
      child_pos = -1
      child_pos = self._child_pos_by_index(node_pos, child_side)
      if self._has_node_at_position(child_pos):
        self._remove(child_pos)
    self._binary_tree[node_pos] = None
    self._size -= 1

  def size(self):
    return self._size
  
  def _position(self, node):
    try:
      pos = self._binary_tree.index(node)
    except ValueError:
      raise NoSuchNodeException("No node:" + node)
    return pos

  def _assure_size(self, pos):
    current_length = len(self._binary_tree)
    if pos >= current_length:
      new_length = 2 * current_length
      i = current_length
      while i < new_length: 
        self._binary_tree.append(None)
        i += 1

  def print_vector(self, message):
    print("\n" + message)
    print(self._binary_tree)

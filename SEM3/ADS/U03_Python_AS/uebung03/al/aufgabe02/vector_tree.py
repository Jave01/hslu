
# HSLU / ICS/AIML : Modul ADS : Algorithmen & Datenstrukturen
# Path   : uebung03/al/aufgabe02
# Version: Sun Oct  1 20:01:13 CEST 2023

import enum
from uebung03.al.aufgabe02.no_such_node_exception import NoSuchNodeException


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
    # TODO: Implement here...
    return self._binary_tree[0]
  
  def set_root(self, root):
    # TODO: Implement here...
    self._binary_tree[0] = root
    
  def parent(self, child):
    # TODO: Implement here...
    pass
    
  def left_child(self, parent):
    # TODO: Implement here...
    pass

  def right_child(self, parent):
    # TODO: Implement here...
    pass

  def is_internal(self, node):
    # TODO: Implement here...
    pass

  def is_external(self, node):
    # TODO: Implement here...
    return not (self.left_child(node) or self.right_child(node))
  
  def is_root(self, node):
    return node == self._binary_tree[0]

  def set_right_child(self, parent, child):
    # TODO: Implement here...
    parent.
  
  def set_left_child(self, parent, child):
    # TODO: Implement here...
    pass
      
  def remove_right_child(self, parent):
    # TODO: Implement here...
    pass

  def remove_left_child(self, parent):
    # TODO: Implement here...
    pass

  def size(self):
    # TODO: Implement here...
    pass

  def print_vector(self, message):
    print("\n" + message)
    print(self._binary_tree)

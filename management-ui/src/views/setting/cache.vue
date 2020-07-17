<template>
  <div>
    <a-tabs defaultActiveKey="delete">
      <a-tab-pane tab="删除缓存" key="delete">
        <cache-delete />
      </a-tab-pane>
      <a-tab-pane tab="刷新缓存" key="update">
        <cache-update />
      </a-tab-pane>
    </a-tabs>
  </div>
</template>
<script>
import { post_form } from '@/api/ajax'
import cache_delete from './cache_delete'
import cache_update from './cache_update'

export default {
  components: {
    'cache-delete': cache_delete,
    'cache-update': cache_update
  },
  data() {
    return {}
  },
  mounted() {
    //
  },
  methods: {}
}
</script>
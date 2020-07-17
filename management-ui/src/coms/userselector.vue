<template>
  <a-select
    labelInValue
    mode="multiple"
    size="large"
    placeholder="搜索用户"
    style="width: 100%"
    icon="user"
    :value="value"
    :filterOption="false"
    :notFoundContent="fetching ? undefined : null"
    @search="search"
    @change="handleChange"
  >
    <a-icon slot="suffixIcon" type="team" />
    <a-spin v-if="fetching" slot="notFoundContent" size="small" />
    <a-select-option v-for="d in data" :key="d.value">
      <a-avatar :src="d.UserImg" size="small">{{d.text}}</a-avatar>
      <span>{{d.text}}</span>
    </a-select-option>
  </a-select>
</template>
<script>
import debounce from 'lodash/debounce'
import { post_form } from '@/api/ajax'

export default {
  data() {
    return {
      data: [],
      value: [],
      fetching: false
    }
  },
  methods: {
    search: debounce(function(value) {
      console.log('fetching user', value)
      this.data = []
      this.fetching = true
      post_form(this, '/api/member/admin/User/SelectUser', {
        q: value,
        role_uid: null
      })
        .then(res => {
          const data = res.Data.map(x => ({
            text: x.NickName || x.UserName,
            value: x.UID,
            UserImg: x.UserImg
          }))
          this.data = data
        })
        .finally(() => {
          this.fetching = false
        })
    }, 800),
    handleChange(val) {
      this.value = val
      this.data = []

      var selected_uids = this.value.map(x => x.key)
      this.$emit('select', selected_uids)
      console.log(selected_uids)
    }
  }
}
</script>
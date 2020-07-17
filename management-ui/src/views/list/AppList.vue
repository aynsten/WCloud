<template>
  <div class="card-list" ref="content">
    <a-list
      :grid="{gutter: 24, lg: 4, md: 2, sm: 1, xs: 1}"
      :dataSource="dataSource"
    >
      <a-list-item slot="renderItem" slot-scope="item">
        <template>
          <a-card :hoverable="true">
            <img
              alt="example"
              :src="item.photosre"
              slot="cover"
            />
            <a-card-meta
              :title="item.title"
              :description="item.description"
            >
            </a-card-meta>
            <template class="ant-card-actions" slot="actions">
              <a-icon type="ellipsis" />
            </template>
          </a-card>
        </template>
      </a-list-item>
    </a-list>
  </div>
</template>

<script>
import { getSmsCaptcha, get2step, getApplist } from '@/api/login'

const dataSource = []
getApplist()
      .then(res => {
        for (let i = 0; i < res.Data.length; i++)
        { 
          dataSource.push({
          id : i,
          photosre : res.Data[i].Logo,
          title : res.Data[i].Name,
          description : res.Data[i].Description
          })
        }
        console.log(dataSource)
      })
      .catch(e => {
        console.log(e)
      })
export default {
data () {
      return {
        dataSource
      }
  }
}
</script>

<style lang="less" scoped>
  .card-avatar {
    width: 48px;
    height: 48px;
    border-radius: 48px;
  }

  .ant-card-actions {
    background: #f7f9fa;
    li {
      float: left;
      text-align: center;
      margin: 12px 0;
      color: rgba(0, 0, 0, 0.45);
      width: 50%;

      &:not(:last-child) {
        border-right: 1px solid #e8e8e8;
      }

      a {
        color: rgba(0, 0, 0, .45);
        line-height: 22px;
        display: inline-block;
        width: 100%;
        &:hover {
          color: #1890ff;
        }
      }
    }
  }

  .new-btn {
    background-color: #fff;
    border-radius: 2px;
    width: 100%;
    height: 188px;
  }

  .meta-content {
    position: relative;
    overflow: hidden;
    text-overflow: ellipsis;
    display: -webkit-box;
    height: 64px;
    -webkit-line-clamp: 3;
    -webkit-box-orient: vertical;
  }
</style>
